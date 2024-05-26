using System.Globalization;
using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class ExpenseOrderEditDTO
    {
        [JsonConstructor]
        public ExpenseOrderEditDTO()
        {

        }
        public ExpenseOrderEditDTO(ExpenseOrder expenseOrder)
        {
            Id = expenseOrder.Id;
            Commentary = expenseOrder.Commentary;
            ExpenseOrderProduct = expenseOrder.ExpenseOrderProduct.ToList().ConvertAll(p => new ExpenseOrderProductDTO(p));
            IsExpense = expenseOrder.IsExpense;
            CultureInfo culture = new CultureInfo("ru-RU");
            DateOfCreate = expenseOrder.DateOfCreate.ToString("d MMMM yyyy 'г.'", culture);
            DateOfExpense = expenseOrder.DateOfExpense?.ToString("d MMMM yyyy 'г.' HH:mm", culture);
            Employee = expenseOrder.Employee.Surname + " " + expenseOrder.Employee.Name;

        }
        public int Id { get; set; }

        public string? Commentary { get; set; }
        public bool IsExpense { get; set; }
        public string? DateOfCreate { get; set; }

        public string? DateOfExpense { get; set; }
        public string? Employee { get; set; }

        public List<ExpenseOrderProductDTO> ExpenseOrderProduct { get; set; }
    }
}
