using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ProductDetails : ReactiveObject
    {
        private int _productId;
        private int? _brandId;
        private int? _manufacturerId;
        private string _countryOfProduction;
        private int? _shelfLife;
        private double? _weight;
        private int? _packageId;
        private int? _typeId;
        private int? _tasteId;
        private int? _fillingId;
        private int? _dietId;

        public int ProductId
        {
            get => _productId;
            set => this.RaiseAndSetIfChanged(ref _productId, value);
        }

        public int? BrandId
        {
            get => _brandId;
            set => this.RaiseAndSetIfChanged(ref _brandId, value);
        }

        public int? ManufacturerId
        {
            get => _manufacturerId;
            set => this.RaiseAndSetIfChanged(ref _manufacturerId, value);
        }

        public string CountryOfProduction
        {
            get => _countryOfProduction;
            set => this.RaiseAndSetIfChanged(ref _countryOfProduction, value);
        }

        public int? ShelfLife
        {
            get => _shelfLife;
            set => this.RaiseAndSetIfChanged(ref _shelfLife, value);
        }

        public double? Weight
        {
            get => _weight;
            set => this.RaiseAndSetIfChanged(ref _weight, value);
        }

        public int? PackageId
        {
            get => _packageId;
            set => this.RaiseAndSetIfChanged(ref _packageId, value);
        }

        public int? TypeId
        {
            get => _typeId;
            set => this.RaiseAndSetIfChanged(ref _typeId, value);
        }

        public int? TasteId
        {
            get => _tasteId;
            set => this.RaiseAndSetIfChanged(ref _tasteId, value);
        }

        public int? FillingId
        {
            get => _fillingId;
            set => this.RaiseAndSetIfChanged(ref _fillingId, value);
        }

        public int? DietId
        {
            get => _dietId;
            set => this.RaiseAndSetIfChanged(ref _dietId, value);
        }
    }
}
