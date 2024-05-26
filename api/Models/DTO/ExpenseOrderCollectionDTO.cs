namespace api.Models.DTO
{
    public class ExpenseOrderCollectionDTO
    {
        public ExpenseOrderCollectionDTO(IEnumerable<ExpenseOrderDTO> expenseOrders,int count)
        {
            ExpenseOrders = expenseOrders;
            Count = count;
        }
        public IEnumerable<ExpenseOrderDTO> ExpenseOrders { get; set; }
        public int Count { get; set; }
    }
}
