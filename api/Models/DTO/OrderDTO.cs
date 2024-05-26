namespace api.Models.DTO
{
    public class OrderDTO
    {
        public OrderDTO( Order order)
        {
            OrderId = order.OrderId;
            Employee = order.Employee.Surname + " " + order.Employee.Name;
            DateOfOrder = order.DateOfOrder;
            DateOfShipment = order.DateOfShipment;
            Commentary = order.Commentary;
            Address = order.Address;
            IsShipment = order.IsShipment;
            Total = order.OrderProduct.Sum(x => x.Quantity * x.Price);
        }
        public int OrderId { get; set; }

        public string Employee { get; set; }

        public DateTime DateOfOrder { get; set; }

        public DateTime? DateOfShipment { get; set; }

        public string? Commentary { get; set; }

        public string Address { get; set; } = null!;

        public bool IsShipment { get; set; }
        public decimal Total { get; set; }
    }
}
