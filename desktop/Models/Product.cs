using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class Product : ReactiveObject
    {
        private decimal _priceOfSale;
        public int ProductId { get; set; }
        public string? ProductNumber { get; set; }
        public long? Barcode { get; set; }
        public int? Count { get; set; }
        public string Name { get; set; } = null!;

        public decimal PriceOfSale
        {
            get => _priceOfSale;
            set => this.RaiseAndSetIfChanged(ref _priceOfSale, value);
        }

        public decimal? MinPriceOfSale { get; set; }

        public decimal? PurchasePrice { get; set; }

        public string? Image { get; set; }
    }
}
