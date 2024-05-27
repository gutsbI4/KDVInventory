using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using desktop.Services;
using desktop.ViewModels;
using Splat;
using System.Linq;

namespace desktop.Views
{
    public partial class AddEditProductWindow : Window
    {
        public AddEditProductWindow()
        {
            InitializeComponent();
            var topLevel = TopLevel.GetTopLevel(this);
            Locator.Current.GetService<IFilePickerService>().RegisterProvider(topLevel);
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
        private bool _mouseDownForWindowMoving = false;
        private PointerPoint _originalPoint;
        private bool _isComboBoxOpened = false;

        private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_mouseDownForWindowMoving) return;

            PointerPoint currentPoint = e.GetCurrentPoint(this);
            Position = new PixelPoint(Position.X + (int)(currentPoint.Position.X - _originalPoint.Position.X),
                Position.Y + (int)(currentPoint.Position.Y - _originalPoint.Position.Y));
        }

        private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (WindowState == WindowState.Maximized || WindowState == WindowState.FullScreen) return;

            _mouseDownForWindowMoving = true;
            _originalPoint = e.GetCurrentPoint(this);
        }

        private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _mouseDownForWindowMoving = false;
        }

        private void ComboBox_DropDownOpened(object? sender, System.EventArgs e)
        {
            _mouseDownForWindowMoving = false;
        }

        private void ComboBox_DropDownClosed_1(object? sender, System.EventArgs e)
        {
            //_mouseDownForWindowMoving = true;
        }
    }
}
