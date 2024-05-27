using api.Data;
using api.Models;
using api.Models.DTO;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReceiptOrderController : Controller
    {
        private readonly KDVDbContext _dbContext;
        public ReceiptOrderController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpGet]
        [Route("GetReceiptOrders")]
        public async Task<ActionResult<ReceiptOrderCollectionDTO>> GetReceiptOrders([FromQuery] OwnerParameters ownerParameters)
        {
            int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            try
            {
                IQueryable<ReceiptOrder> receiptOrdersQuery =  _dbContext.ReceiptOrder.Include(p => p.Employee)
                     .ThenInclude(p => p.EmployeeNavigation)
                     .Include(p => p.ReceiptOrderProduct).OrderByDescending(x=>x.Id);
                if (!String.IsNullOrEmpty(ownerParameters.SearchString))
                {
                    string search = ownerParameters.SearchString.ToLower();
                    receiptOrdersQuery = receiptOrdersQuery.Where(b=>b.Id.ToString().Contains(search) ||
                    b.ReceiptOrderProduct.Select(p=>p.Product.Name).Contains(search) ||
                    b.Commentary.Contains(search) || search == "");
                }
                int count = await receiptOrdersQuery.CountAsync();
                var receiptorders = await receiptOrdersQuery.Skip((ownerParameters.PageNumber - 1) * ownerParameters.SizePage)
                    .Take(ownerParameters.SizePage).ToListAsync();
                await AuditLogger.AddAuditRecord(_dbContext, idUser, "Получил список приходов.");

                return Ok(new ReceiptOrderCollectionDTO(receiptorders.ConvertAll(x=> new ReceiptOrderDTO(x)),count));
            }
            catch (JsonException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("SaveReceiptOrder")]
        public async Task<ActionResult<int>> SaveReceiptOrder([FromBody] ReceiptOrderEditDTO receiptOrderEditDTO)
        {
            int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            try
            {
                ReceiptOrder receiptOrder;
                if (receiptOrderEditDTO.Id == 0)
                {
                    receiptOrder = new ReceiptOrder();
                    _dbContext.ReceiptOrder.Add(receiptOrder);
                }
                else
                {
                    receiptOrder = await _dbContext.ReceiptOrder
                        .Include(ro => ro.ReceiptOrderProduct)
                        .FirstOrDefaultAsync(x => x.Id == receiptOrderEditDTO.Id);

                    if (receiptOrder == null)
                    {
                        return NotFound("ReceiptOrder not found");
                    }

                    _dbContext.ReceiptOrderProduct.RemoveRange(receiptOrder.ReceiptOrderProduct);
                }

                receiptOrder.Commentary = receiptOrderEditDTO.Commentary;
                if (receiptOrderEditDTO.IsReceipt && !receiptOrder.IsReceipt) receiptOrder.DateOfReceipt = DateTime.Now;
                else if(!receiptOrderEditDTO.IsReceipt) receiptOrder.DateOfReceipt = null;
                receiptOrder.IsReceipt = receiptOrderEditDTO.IsReceipt;
                receiptOrder.EmployeeId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                if (receiptOrderEditDTO.Id == 0) receiptOrder.DateOfCreate = DateTime.Now;

                List<ReceiptOrderProduct> receiptOrderProducts = new List<ReceiptOrderProduct>();

                foreach (var receipt in receiptOrderEditDTO.ReceiptOrderProduct)
                {
                    receiptOrderProducts.Add(new ReceiptOrderProduct
                    {
                        ProductId = receipt.Product.ProductId,
                        PurchasePrice = receipt.PurchasePrice,
                        Quantity = receipt.Quantity
                    });
                    var product = await _dbContext.Product.FirstAsync(x => x.ProductId == receipt.Product.ProductId);
                    product.PriceOfSale = receipt.Product.PriceOfSale;
                    product.PurchasePrice = receipt.PurchasePrice;
                }

                receiptOrder.ReceiptOrderProduct = receiptOrderProducts;

                await _dbContext.SaveChangesAsync();

                await AuditLogger.AddAuditRecord(_dbContext, idUser, $"Работал с приходом товаров. ID:{receiptOrder.Id}");
                return Ok(receiptOrder.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetReceiptOrderEdit/{id}")]
        public async Task<ActionResult<ReceiptOrderEditDTO>> GetReceiptOrderEdit(int id)
        {
            try
            {
                var receiptOrder = await _dbContext.ReceiptOrder
                .Include(ro => ro.ReceiptOrderProduct)
                    .ThenInclude(rop => rop.Product)
                        .ThenInclude(p => p.OrderProduct)
                            .ThenInclude(op => op.Order)
                .Include(ro => ro.ReceiptOrderProduct)
                    .ThenInclude(rop => rop.Product)
                        .ThenInclude(p => p.ExpenseOrderProduct)
                            .ThenInclude(eop => eop.ExpenseOrder)
                .Include(ro => ro.Employee)
                .FirstOrDefaultAsync(ro => ro.Id == id);

                if (receiptOrder == null) return NotFound();
                return Ok(new ReceiptOrderEditDTO(receiptOrder));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
