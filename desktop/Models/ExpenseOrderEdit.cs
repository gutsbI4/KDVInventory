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
    public class ExpenseOrderEdit : ReactiveObject
    {
        private ObservableCollection<ExpenseOrderProduct> _expenseOrderProducts;
        private bool _isExpense;
        private decimal _total;

        public int Id { get; set; }

        public string? Commentary { get; set; }
        public bool IsExpense
        {
            get => _isExpense;
            set => this.RaiseAndSetIfChanged(ref _isExpense, value);
        }
        public decimal Total
        {
            get => _total;
            set => this.RaiseAndSetIfChanged(ref _total, value);
        }
        public string? DateOfCreate { get; set; }

        public string? DateOfExpense { get; set; }
        public string? Employee { get; set; }
        public ObservableCollection<ExpenseOrderProduct> ExpenseOrderProduct
        {
            get => _expenseOrderProducts;
            set
            {
                if (_expenseOrderProducts != null)
                {
                    foreach (var item in _expenseOrderProducts)
                    {
                        item.PropertyChanged -= OnExpenseOrderProductPropertyChanged;
                    }
                }

                _expenseOrderProducts = value;

                if (_expenseOrderProducts != null)
                {
                    foreach (var item in _expenseOrderProducts)
                    {
                        item.PropertyChanged += OnExpenseOrderProductPropertyChanged;
                    }
                }

                this.RaisePropertyChanged();
            }
        }
        public ExpenseOrderEdit()
        {
            _expenseOrderProducts = new ObservableCollection<ExpenseOrderProduct>();

            ExpenseOrderProduct.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (ExpenseOrderProduct newItem in args.NewItems)
                    {
                        newItem.PropertyChanged += OnExpenseOrderProductPropertyChanged;
                    }
                }
                if (args.OldItems != null)
                {
                    foreach (ExpenseOrderProduct oldItem in args.OldItems)
                    {
                        oldItem.PropertyChanged -= OnExpenseOrderProductPropertyChanged;
                    }
                }
            };
            this.WhenAnyValue(x => x.ExpenseOrderProduct).Subscribe(_ => RecalculateTotals());
        }
        private void OnExpenseOrderProductPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Quantity" || e.PropertyName == "Price")
            {
                RecalculateTotals();
            }
        }


        private void RecalculateTotals()
        {
            Total = _expenseOrderProducts.Sum(x => x.Quantity * x.Price);
        }
    
    }
}
