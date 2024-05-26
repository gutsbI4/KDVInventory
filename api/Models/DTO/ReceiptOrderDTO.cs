namespace api.Models.DTO
{
    public class ReceiptOrderDTO
    {
        public ReceiptOrderDTO(ReceiptOrder receiptOrder)
        {
            Id = receiptOrder.Id;
            DateOfCreate = receiptOrder.DateOfCreate;
            DateOfReceipt = receiptOrder.DateOfReceipt;
            IsReceipt = receiptOrder.IsReceipt;
            Commentary = receiptOrder.Commentary;
            Employee = receiptOrder.Employee.Surname + " " + receiptOrder.Employee.Name;
            Total = receiptOrder.ReceiptOrderProduct.Sum(x=>x.Quantity * x.PurchasePrice);
        }
        public int Id { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime? DateOfReceipt { get; set; }

        public bool IsReceipt { get; set; }

        public string? Commentary { get; set; }

        public string Employee { get; set; }
        public decimal? Total { get;set; }
    }
}
