namespace api.Models.DTO
{
    public class ReceiptOrderCollectionDTO
    {
        public ReceiptOrderCollectionDTO(IEnumerable<ReceiptOrderDTO> receiptOrders, int count)
        {
            ReceiptOrders = receiptOrders;
            Count = count;
        }
        public IEnumerable<ReceiptOrderDTO> ReceiptOrders { get; set; }
        public int Count { get; set; }
    }
}
