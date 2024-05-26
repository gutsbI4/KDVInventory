using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class EnergyValue : ReactiveObject
    {
        private int _productId;
        private double? _proteins;
        private double? _fats;
        private double? _carbs;
        private double? _calories;

        public int ProductId
        {
            get => _productId;
            set => this.RaiseAndSetIfChanged(ref _productId, value);
        }

        public double? Proteins
        {
            get => _proteins;
            set => this.RaiseAndSetIfChanged(ref _proteins, value);
        }

        public double? Fats
        {
            get => _fats;
            set => this.RaiseAndSetIfChanged(ref _fats, value);
        }

        public double? Carbs
        {
            get => _carbs;
            set => this.RaiseAndSetIfChanged(ref _carbs, value);
        }

        public double? Calories
        {
            get => _calories;
            set => this.RaiseAndSetIfChanged(ref _calories, value);
        }
    }
}
