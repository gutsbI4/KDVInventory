using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace api.Models.DTO
{
    public class OrderEditDTO
    {
        [JsonConstructor]
        public OrderEditDTO()
        {
            
        }
        public OrderEditDTO(Order order)
        {
            Id = order.OrderId;
            Commentary = order.Commentary;
            IsShipment = order.IsShipment;
            DateOfOrder = order.DateOfOrder.ToString("d MMMM yyyy 'г.'", new CultureInfo("ru-RU"));
            DateOfShipment = order.DateOfShipment;
            Employee = order.Employee.Surname + " " + order.Employee.Name;
            Address = order.Address;
            OrderProduct = order.OrderProduct.ToList().ConvertAll(p => new OrderProductDTO(p));
        }
        public int Id { get; set; }

        public string? Commentary { get; set; }
        public bool IsShipment { get; set; }
        public string? DateOfOrder { get; set; }

        public DateTime? DateOfShipment { get; set; }
        public string? Employee { get; set; }
        public string Address { get; set; }
        public List<OrderProductDTO> OrderProduct { get;set; }
    }
}
