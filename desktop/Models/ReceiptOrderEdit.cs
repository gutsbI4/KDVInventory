using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace desktop.Models
{
    public class ReceiptOrderEdit : ReactiveObject
    {
        private ObservableCollection<ReceiptOrderProduct> _receiptOrderProducts;
        private bool _isReceipt;
        private decimal _total;
        private int _unitsOfGoods;

        public int Id { get; set; }

        public string? Commentary { get; set; }
        public bool IsReceipt
        {
            get => _isReceipt;
            set => this.RaiseAndSetIfChanged(ref _isReceipt, value);
        }
        public decimal Total
        {
            get => _total;
            set => this.RaiseAndSetIfChanged(ref _total, value);
        }
        public int UnitsOfGoods
        {
            get => _unitsOfGoods;
            set => this.RaiseAndSetIfChanged(ref _unitsOfGoods, value);
        }
        public string? DateOfCreate { get; set; }

        public string? DateOfReceipt { get; set; }
        public string? Employee { get; set; }
        public ObservableCollection<ReceiptOrderProduct> ReceiptOrderProduct
        {
            get => _receiptOrderProducts;
            set
            {
                if (_receiptOrderProducts != null)
                {
                    foreach (var item in _receiptOrderProducts)
                    {
                        item.PropertyChanged -= OnReceiptOrderProductPropertyChanged;
                    }
                }

                _receiptOrderProducts = value;

                if (_receiptOrderProducts != null)
                {
                    foreach (var item in _receiptOrderProducts)
                    {
                        item.PropertyChanged += OnReceiptOrderProductPropertyChanged;
                    }
                }

                this.RaisePropertyChanged();
            }
        }
        public ReceiptOrderEdit()
        {
            _receiptOrderProducts = new ObservableCollection<ReceiptOrderProduct>();

            ReceiptOrderProduct.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (ReceiptOrderProduct newItem in args.NewItems)
                    {
                        newItem.PropertyChanged += OnReceiptOrderProductPropertyChanged;
                    }
                }
                if (args.OldItems != null)
                {
                    foreach (ReceiptOrderProduct oldItem in args.OldItems)
                    {
                        oldItem.PropertyChanged -= OnReceiptOrderProductPropertyChanged;
                    }
                }
            };
            this.WhenAnyValue(x => x.ReceiptOrderProduct).Subscribe(_=>RecalculateTotals());
        }
        private void OnReceiptOrderProductPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Quantity" || e.PropertyName == "PurchasePrice")
            {
                RecalculateTotals();
            }
        }


        private void RecalculateTotals()
        {
            Total = _receiptOrderProducts.Sum(x => x.Quantity * x.PurchasePrice);
            UnitsOfGoods = _receiptOrderProducts.Sum(x => x.Quantity);
        }
    }
}
