using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class OrderEdit : ReactiveObject
    {
        private ObservableCollection<OrderProduct> _orderProducts;
        private bool _isShipment;
        private decimal _total;
        private DateTime? _shipmentDate;
        private TimeSpan? _shipmentTime;
        private DateTime? _dateOfShipment;

        public int Id { get; set; }

        public string? Commentary { get; set; }
        public bool IsShipment
        {
            get => _isShipment;
            set => this.RaiseAndSetIfChanged(ref _isShipment, value);
        }
        public DateTime? ShipmentDate
        {
            get => _shipmentDate;
            set => this.RaiseAndSetIfChanged(ref _shipmentDate, value);
        }
        public TimeSpan? ShipmentTime
        {
            get => _shipmentTime;
            set => this.RaiseAndSetIfChanged(ref _shipmentTime, value);
        }

        public decimal Total
        {
            get => _total;
            set => this.RaiseAndSetIfChanged(ref _total, value);
        }
        public string? DateOfOrder { get; set; }

        public DateTime? DateOfShipment
        {
            get => _dateOfShipment;
            set => this.RaiseAndSetIfChanged(ref _dateOfShipment, value);
        }
        public string? Employee { get; set; }
        public string Address { get; set; }
        
        public ObservableCollection<OrderProduct> OrderProduct
        {
            get => _orderProducts;
            set
            {
                if (_orderProducts != null)
                {
                    foreach (var item in _orderProducts)
                    {
                        item.PropertyChanged -= OnOrderProductPropertyChanged;
                    }
                }

                _orderProducts = value;

                if (_orderProducts != null)
                {
                    foreach (var item in _orderProducts)
                    {
                        item.PropertyChanged += OnOrderProductPropertyChanged;
                    }
                }

                this.RaisePropertyChanged();
            }
        }
        public OrderEdit()
        {
            _orderProducts = new ObservableCollection<OrderProduct>();

            OrderProduct.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (OrderProduct newItem in args.NewItems)
                    {
                        newItem.PropertyChanged += OnOrderProductPropertyChanged;
                    }
                }
                if (args.OldItems != null)
                {
                    foreach (OrderProduct oldItem in args.OldItems)
                    {
                        oldItem.PropertyChanged -= OnOrderProductPropertyChanged;
                    }
                }
            };
            this.WhenAnyValue(x => x.DateOfShipment).Subscribe(_ =>
            {
                if(DateOfShipment == null)
                {
                    ShipmentDate = DateTime.Now;
                    ShipmentTime = new TimeSpan(9, 0, 0);
                }
                else
                {
                    ShipmentDate = new DateTime(DateOfShipment.Value.Year, DateOfShipment.Value.Month, DateOfShipment.Value.Day);
                    ShipmentTime = new TimeSpan(DateOfShipment.Value.Hour, DateOfShipment.Value.Minute, DateOfShipment.Value.Second);
                }
            });
            this.WhenAnyValue(x => x.OrderProduct).Subscribe(_ => RecalculateTotals());
            this.WhenAnyValue(x => x.ShipmentDate, y => y.ShipmentTime).Subscribe(_ =>
            {
                DateOfShipment = new DateTime(ShipmentDate.Value.Year, ShipmentDate.Value.Month, ShipmentDate.Value.Day, ShipmentTime.Value.Hours, ShipmentTime.Value.Minutes, ShipmentTime.Value.Seconds);
            });
            
        }
        private void OnOrderProductPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Quantity" || e.PropertyName == "Price")
            {
                RecalculateTotals();
            }
        }


        private void RecalculateTotals()
        {
            Total = _orderProducts.Sum(x => x.Quantity * x.Price);
        }
    }
}
