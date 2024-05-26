namespace api.Models.DTO
{
    public class OrderCollectionDTO
    {
        public OrderCollectionDTO(IEnumerable<OrderDTO> orders, int count)
        {
            Orders = orders;
            Count = count;
        }
        public IEnumerable<OrderDTO> Orders { get; set; }
        public int Count { get; set; }
    }
}
