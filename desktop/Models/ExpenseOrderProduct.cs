using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ExpenseOrderProduct : ReactiveObject
    {
        private int _quantity;
        private decimal _price;
        private decimal _sum;
        private Product _product;
        public ExpenseOrderProduct()
        {
            this.WhenAnyValue(x => x.Price, x => x.Quantity).Subscribe(_ =>
            {
                Sum = Price * Quantity;
            });
        }
        public Product Product
        {
            get => _product;
            set => this.RaiseAndSetIfChanged(ref _product, value);
        }

        public int ExpenseOrderId { get; set; }

        public int Quantity
        {
            get => _quantity;
            set => this.RaiseAndSetIfChanged(ref _quantity, value);
        }

        public decimal Price
        {
            get => _price;
            set => this.RaiseAndSetIfChanged(ref _price, value);
        }
        public decimal Sum
        {
            get => _sum;
            set => this.RaiseAndSetIfChanged(ref _sum, value);
        }
    }
}
