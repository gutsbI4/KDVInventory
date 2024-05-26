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
    public class ExpenseOrderController : Controller
    {
        private readonly KDVDbContext _dbContext;
        public ExpenseOrderController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpGet]
        [Route("GetExpenseOrders")]
        public async Task<ActionResult<ExpenseOrderCollectionDTO>> GetExpenseOrders([FromQuery] OwnerParameters ownerParameters)
        {
            int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            try
            {
                IQueryable<ExpenseOrder> expenseOrdersQuery = _dbContext.ExpenseOrder.Include(p => p.Employee)
                     .ThenInclude(p => p.EmployeeNavigation)
                     .Include(p => p.ExpenseOrderProduct);
                if (!String.IsNullOrEmpty(ownerParameters.SearchString))
                {
                    string search = ownerParameters.SearchString.ToLower();
                    expenseOrdersQuery = expenseOrdersQuery.Where(b => b.Id.ToString().Contains(search) ||
                    b.ExpenseOrderProduct.Select(p => p.Product.Name).Contains(search) ||
                    b.Commentary.Contains(search) || search == "");
                }
                int count = await expenseOrdersQuery.CountAsync();
                var expenseOrders = await expenseOrdersQuery.Skip((ownerParameters.PageNumber - 1) * ownerParameters.SizePage)
                    .Take(ownerParameters.SizePage).ToListAsync();

                await AuditLogger.AddAuditRecord(_dbContext, idUser, "Получил список списаний.");
                return Ok(new ExpenseOrderCollectionDTO(expenseOrders.ConvertAll(x => new ExpenseOrderDTO(x)), count));

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
        [Route("SaveExpenseOrder")]
        public async Task<ActionResult<int>> SaveExpenseOrder([FromBody] ExpenseOrderEditDTO expenseOrderEditDTO)
        {
            int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            try
            {
                ExpenseOrder expenseOrder;
                if (expenseOrderEditDTO.Id == 0)
                {
                    expenseOrder = new ExpenseOrder();
                    _dbContext.ExpenseOrder.Add(expenseOrder);
                }
                else
                {
                    expenseOrder = await _dbContext.ExpenseOrder
                        .Include(ro => ro.ExpenseOrderProduct)
                        .FirstOrDefaultAsync(x => x.Id == expenseOrderEditDTO.Id);

                    if (expenseOrder == null)
                    {
                        return NotFound();
                    }

                    _dbContext.ExpenseOrderProduct.RemoveRange(expenseOrder.ExpenseOrderProduct);
                }

                expenseOrder.Commentary = expenseOrderEditDTO.Commentary;
                expenseOrder.IsExpense = expenseOrderEditDTO.IsExpense;
                if (expenseOrderEditDTO.IsExpense) expenseOrder.DateOfExpense = DateTime.Now;
                else expenseOrder.DateOfExpense = null;
                expenseOrder.EmployeeId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                if (expenseOrderEditDTO.Id == 0) expenseOrder.DateOfCreate = DateTime.Now;

                List<ExpenseOrderProduct> expenseOrderProducts = new List<ExpenseOrderProduct>();

                foreach (var expense in expenseOrderEditDTO.ExpenseOrderProduct)
                {
                    expenseOrderProducts.Add(new ExpenseOrderProduct
                    {
                        ProductId = expense.Product.ProductId,
                        Price = expense.Price,
                        Quantity = expense.Quantity
                    });
                }

                expenseOrder.ExpenseOrderProduct = expenseOrderProducts;

                await _dbContext.SaveChangesAsync();

                await AuditLogger.AddAuditRecord(_dbContext, idUser, $"Работал со списанием товаров. ID: {expenseOrder.Id}");

                return Ok(expenseOrder.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetExpenseOrderEdit/{id}")]
        public async Task<ActionResult<ExpenseOrderEditDTO>> GetExpenseOrderEdit(int id)
        {
            try
            {
                var expenseOrder = await _dbContext.ExpenseOrder
                .Include(ro => ro.ExpenseOrderProduct)
                    .ThenInclude(rop => rop.Product)
                        .ThenInclude(p => p.OrderProduct)
                            .ThenInclude(op => op.Order)
                .Include(ro => ro.ExpenseOrderProduct)
                    .ThenInclude(rop => rop.Product)
                        .ThenInclude(p => p.ReceiptOrderProduct)
                            .ThenInclude(eop => eop.ReceiptOrder)
                .Include(ro => ro.Employee)
                .FirstOrDefaultAsync(ro => ro.Id == id);
                return Ok(new ExpenseOrderEditDTO(expenseOrder));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
