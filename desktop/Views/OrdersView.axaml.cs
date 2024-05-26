using Avalonia.Controls;
using Avalonia.Media;
using desktop.Models;
using desktop.ViewModels;
using System;

namespace desktop.Views
{
    public partial class OrdersView : UserControl
    {
        public OrdersView()
        {
            InitializeComponent();
        }
        private void DataGrid_CellPointerPressed(object? sender, Avalonia.Controls.DataGridCellPointerPressedEventArgs e)
        {
            var vm = DataContext as OrdersViewModel;
            if (vm != null)
            {
                var order = e.Cell.DataContext as Order;
                if (order == null) return;
                vm.EditOrderCommand.Execute(order.OrderId).Subscribe();
            }
        }

        private void DataGrid_LoadingRow(object? sender, Avalonia.Controls.DataGridRowEventArgs e)
        {
            var row = e.Row;
            var item = e.Row.DataContext as Order;
            if (!item.IsShipment) row.Background = new SolidColorBrush(Color.FromArgb(105, 255, 6, 10));
            else row.Background = new SolidColorBrush(Color.FromArgb(103, 0, 195,32));
        }
    }
}
