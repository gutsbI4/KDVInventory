using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ProductEdit : ReactiveObject
    {
        private string? _productNumber;
        private long? _barcode;
        private string _name = null!;
        private string? _description;
        private decimal _priceOfSale;
        private decimal? _minPriceOfSale;
        private decimal? _purchasePrice;
        private int? _lowStockThreshold;
        private int? _categoryId;
        private int? _priceUnitId;
        private IEnumerable<ProductImage>? _productImage;
        private ProductDetails? _details;
        private EnergyValue? _energy;

        public int ProductId { get; set; }

        [StringLength(10,ErrorMessage ="Слишком длинный артикул")]
        public string? ProductNumber
        {
            get => _productNumber;
            set => this.RaiseAndSetIfChanged(ref _productNumber, value);
        }
        public long? Barcode
        {
            get => _barcode;
            set => this.RaiseAndSetIfChanged(ref _barcode, value);
        }

        [Required(ErrorMessage = "Укажите наименование товара")]
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string? Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        [Required(ErrorMessage = "Обязательно укажите цену продажи")]
        public decimal PriceOfSale
        {
            get => _priceOfSale;
            set => this.RaiseAndSetIfChanged(ref _priceOfSale, value);
        }

        public decimal? MinPriceOfSale
        {
            get => _minPriceOfSale;
            set => this.RaiseAndSetIfChanged(ref _minPriceOfSale, value);
        }

        public decimal? PurchasePrice
        {
            get => _purchasePrice;
            set => this.RaiseAndSetIfChanged(ref _purchasePrice, value);
        }

        public int? LowStockThreshold
        {
            get => _lowStockThreshold;
            set => this.RaiseAndSetIfChanged(ref _lowStockThreshold, value);
        }

        public int? CategoryId
        {
            get => _categoryId;
            set => this.RaiseAndSetIfChanged(ref _categoryId, value);
        }

        public int? PriceUnitId
        {
            get => _priceUnitId;
            set => this.RaiseAndSetIfChanged(ref _priceUnitId, value);
        }

        public IEnumerable<ProductImage>? ProductImage
        {
            get => _productImage;
            set => this.RaiseAndSetIfChanged(ref _productImage, value);
        }

        public ProductDetails? Details
        {
            get => _details;
            set => this.RaiseAndSetIfChanged(ref _details, value);
        }

        public EnergyValue? Energy
        {
            get => _energy;
            set => this.RaiseAndSetIfChanged(ref _energy, value);
        }
    }
}
