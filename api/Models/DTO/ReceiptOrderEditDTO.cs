using System.Globalization;
using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class ReceiptOrderEditDTO
    {
        [JsonConstructor]
        public ReceiptOrderEditDTO()
        {
            
        }
        public ReceiptOrderEditDTO(ReceiptOrder receiptOrder)
        {
            Id = receiptOrder.Id;
            Commentary = receiptOrder.Commentary;
            ReceiptOrderProduct = receiptOrder.ReceiptOrderProduct.ToList().ConvertAll(p=>new ReceiptOrderProductDTO(p));
            IsReceipt = receiptOrder.IsReceipt;
            CultureInfo culture = new CultureInfo("ru-RU");
            DateOfCreate = receiptOrder.DateOfCreate.ToString("d MMMM yyyy 'г.'", culture);
            DateOfReceipt = receiptOrder.DateOfReceipt?.ToString("d MMMM yyyy 'г.' HH:mm", culture);
            Employee = receiptOrder.Employee.Surname + " " + receiptOrder.Employee.Name;

        }
        public int Id { get; set; }

        public string? Commentary { get; set; }
        public bool IsReceipt { get; set; }
        public string? DateOfCreate { get; set; }

        public string? DateOfReceipt { get; set; }
        public string? Employee { get; set; }

        public List<ReceiptOrderProductDTO> ReceiptOrderProduct { get; set; }
    }
}
