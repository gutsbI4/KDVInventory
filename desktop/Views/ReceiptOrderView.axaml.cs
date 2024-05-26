using Avalonia.Controls;
using Avalonia.Media;
using desktop.Models;
using desktop.ViewModels;
using System;

namespace desktop.Views
{
    public partial class ReceiptOrderView : UserControl
    {
        public ReceiptOrderView()
        {
            InitializeComponent();
        }
        private void DataGrid_CellPointerPressed(object? sender, Avalonia.Controls.DataGridCellPointerPressedEventArgs e)
        {
            var vm = DataContext as ReceiptOrderViewModel;
            if (vm != null)
            {
                var receiptOrder = e.Cell.DataContext as ReceiptOrder;
                if (receiptOrder == null) return;
                vm.EditReceiptOrderCommand.Execute(receiptOrder.Id).Subscribe();
            }
        }

        private void DataGrid_LoadingRow(object? sender, Avalonia.Controls.DataGridRowEventArgs e)
        {
            var row = e.Row;
            var item = e.Row.DataContext as ReceiptOrder;
            if (!item.IsReceipt) row.Background = new SolidColorBrush(Color.FromArgb(105, 255, 6, 10));
            else row.Background = new SolidColorBrush(Color.FromArgb(103, 0, 195, 32));
        }
    }
}
