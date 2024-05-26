using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ReceiptOrderProduct : ReactiveObject
    {
        private int _quantity;
        private decimal _purchasePrice;
        private decimal _sum;
        private float _percentMarkup;
        private Product _product;
        public ReceiptOrderProduct()
        {
            this.WhenAnyValue(x=>x.PurchasePrice, x => x.Quantity).Subscribe(_ =>
            {
                Sum = PurchasePrice * Quantity;
            });
            this.WhenAnyValue(x => x.PurchasePrice, x => x.Product.PriceOfSale).Subscribe(_ =>
            {
                try
                {
                    PercentMarkup = (float)((Product.PriceOfSale / PurchasePrice - 1) * 100);
                }
                catch (Exception)
                {
                    return;
                }
                
            });
        }
        public Product Product
        {
            get => _product;
            set => this.RaiseAndSetIfChanged(ref _product, value);
        }

        public int ReceiptOrderId { get; set; }

        public int Quantity
        {
            get => _quantity;
            set => this.RaiseAndSetIfChanged(ref _quantity, value);
        }

        public decimal PurchasePrice
        {
            get => _purchasePrice;
            set => this.RaiseAndSetIfChanged(ref _purchasePrice, value);
        }
        public decimal Sum
        {
            get => _sum;
            set => this.RaiseAndSetIfChanged(ref _sum, value);
        }
        public float PercentMarkup
        {
            get => _percentMarkup;
            set => this.RaiseAndSetIfChanged(ref _percentMarkup, value);
        }
    }
}
