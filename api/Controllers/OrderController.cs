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
    public class OrderController : Controller
    {
        private readonly KDVDbContext _dbContext;
        public OrderController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpGet]
        [Route("GetOrders")]
        public async Task<ActionResult<OrderCollectionDTO>> GetOrders([FromQuery] OwnerParameters ownerParameters)
        {
            int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            try
            {
                IQueryable<Order> ordersQuery = _dbContext.Order.Include(p => p.Employee)
                     .ThenInclude(p => p.EmployeeNavigation)
                     .Include(p => p.OrderProduct).OrderByDescending(x=>x.OrderId);
                if (!String.IsNullOrEmpty(ownerParameters.SearchString))
                {
                    string search = ownerParameters.SearchString.ToLower();
                    ordersQuery = ordersQuery.Where(b => b.OrderId.ToString().Contains(search) ||
                    b.OrderProduct.Select(p => p.Product.Name).Contains(search) ||
                    b.Commentary.Contains(search) || search == "");
                }
                int count = await ordersQuery.CountAsync();
                var orders = await ordersQuery.Skip((ownerParameters.PageNumber - 1) * ownerParameters.SizePage)
                    .Take(ownerParameters.SizePage).ToListAsync();

                await AuditLogger.AddAuditRecord(_dbContext, idUser, "Получил список заказов.");

                return Ok(new OrderCollectionDTO(orders.ConvertAll(x => new OrderDTO(x)), count));
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
        [Route("SaveOrder")]
        public async Task<ActionResult<int>> SaveOrder([FromBody] OrderEditDTO orderEditDTO)
        {
            int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            try
            {
                Order order;
                if (orderEditDTO.Id == 0)
                {
                    order = new Order();
                    _dbContext.Order.Add(order);
                }
                else
                {
                    order = await _dbContext.Order
                        .Include(ro => ro.OrderProduct)
                        .FirstOrDefaultAsync(x => x.OrderId == orderEditDTO.Id);

                    if (order == null)
                    {
                        return NotFound("Order not found");
                    }

                    _dbContext.OrderProduct.RemoveRange(order.OrderProduct);
                }

                order.Commentary = orderEditDTO.Commentary;
                order.DateOfShipment = orderEditDTO.DateOfShipment;
                order.IsShipment = orderEditDTO.IsShipment;
                order.Address = orderEditDTO.Address;
                order.EmployeeId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                if (orderEditDTO.Id == 0) order.DateOfOrder = DateTime.Now;

                List<OrderProduct> orderProducts = new List<OrderProduct>();

                foreach (var ordere in orderEditDTO.OrderProduct)
                {
                    orderProducts.Add(new OrderProduct
                    {
                        ProductId = ordere.Product.ProductId,
                        Price = ordere.Price,
                        Quantity = ordere.Quantity
                    });
                }

                order.OrderProduct = orderProducts;

                await AuditLogger.AddAuditRecord(_dbContext, idUser, $"Оформил заказ. ID: {order.OrderId}");

                await _dbContext.SaveChangesAsync();

                return Ok(order.OrderId);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetOrderEdit/{id}")]
        public async Task<ActionResult<OrderEditDTO>> GetOrderEdit(int id)
        {
            try
            {
                var order = await _dbContext.Order
                .Include(ro => ro.OrderProduct)
                    .ThenInclude(rop => rop.Product)
                        .ThenInclude(p => p.ReceiptOrderProduct)
                            .ThenInclude(op => op.ReceiptOrder)
                .Include(ro => ro.OrderProduct)
                    .ThenInclude(rop => rop.Product)
                        .ThenInclude(p => p.ExpenseOrderProduct)
                            .ThenInclude(eop => eop.ExpenseOrder)
                .Include(ro => ro.Employee)
                .FirstOrDefaultAsync(ro => ro.OrderId == id);
                if (order == null) return NotFound();
                return Ok(new OrderEditDTO(order));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
