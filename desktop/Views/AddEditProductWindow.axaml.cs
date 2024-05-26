using Avalonia.Controls;
using Avalonia.Interactivity;
using desktop.ViewModels;

namespace desktop.Views
{
    public partial class AddEditProductWindow : Window
    {
        public AddEditProductWindow()
        {
            InitializeComponent();
        }
        
        public void Next(object source, RoutedEventArgs args)
        {
            slides.Next();
        }

        public void Previous(object source, RoutedEventArgs args)
        {
            slides.Previous();
        }

        private void NumericUpDown_ValueChanged(object? sender, Avalonia.Controls.NumericUpDownValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Button_Barcode.IsVisible = false;
            }
            else Button_Barcode.IsVisible = true;
        }
    }
}
