using Avalonia.Controls;
using Avalonia.Media;
using desktop.Models;
using desktop.ViewModels;
using ReactiveUI;
using System;
using System.Collections;
using System.Linq;

namespace desktop.Views
{
    public partial class ExpenseOrdersView : UserControl
    {
        public ExpenseOrdersView()
        {
            InitializeComponent();
        }
        private void DataGrid_CellPointerPressed(object? sender, Avalonia.Controls.DataGridCellPointerPressedEventArgs e)
        {
            var vm = DataContext as ExpenseOrdersViewModel;
            if (vm != null)
            {
                var expenseOrder = e.Cell.DataContext as ExpenseOrder;
                if (expenseOrder == null) return;
                vm.EditExpenseOrderCommand.Execute(expenseOrder.Id).Subscribe();
            }
        }

        private void DataGrid_LoadingRow(object? sender, Avalonia.Controls.DataGridRowEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            var vm = DataContext as ExpenseOrdersViewModel;
        }

        private void DataGrid_LoadingRow_1(object? sender, Avalonia.Controls.DataGridRowEventArgs e)
        {
            var row = e.Row;
            var item = e.Row.DataContext as ExpenseOrder;
            if (!item.IsExpense) row.Background = new SolidColorBrush(Color.FromArgb(105, 255, 6, 10));
            else row.Background = new SolidColorBrush(Color.FromArgb(103, 0, 195, 32));
        }
    }
}
