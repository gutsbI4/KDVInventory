using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using desktop.Models;
using desktop.ViewModels;
using System;

namespace desktop.Views
{
    public partial class ProductsView : UserControl
    {
        public ProductsView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_ScrollChanged(object? sender, Avalonia.Controls.ScrollChangedEventArgs e)
        {
            var scrollbar = sender as ScrollViewer;
            if (scrollbar.Offset.Y > 200) buttonUp.IsVisible = true;
            else buttonUp.IsVisible=false;
        }
        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            scrollViewerProducts.Offset = scrollViewerProducts.Offset.WithY(0);
        }
    }
}
