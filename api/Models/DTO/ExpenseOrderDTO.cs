namespace api.Models.DTO
{
    public class ExpenseOrderDTO
    {
        public ExpenseOrderDTO(ExpenseOrder expenseOrder)
        {
            Id = expenseOrder.Id;
            DateOfCreate = expenseOrder.DateOfCreate;
            DateOfExpense = expenseOrder.DateOfExpense;
            IsExpense = expenseOrder.IsExpense;
            Commentary = expenseOrder.Commentary;
            Employee = expenseOrder.Employee.Surname + " " + expenseOrder.Employee.Name;
            Total = expenseOrder.ExpenseOrderProduct.Sum(x => x.Quantity * x.Price);
        }
        public int Id { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime? DateOfExpense { get; set; }

        public bool IsExpense { get; set; }

        public string? Commentary { get; set; }

        public string Employee { get; set; }
        public decimal? Total { get; set; }
    }
}
