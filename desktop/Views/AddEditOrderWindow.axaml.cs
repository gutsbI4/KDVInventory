using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using desktop.Models;
using desktop.Services;
using desktop.ViewModels;
using Splat;

namespace desktop.Views
{
    public partial class AddEditOrderWindow : Window
    {
        public AddEditOrderWindow()
        {
            InitializeComponent();
            var topLevel = TopLevel.GetTopLevel(this);
            Locator.Current.GetService<IFilePickerService>().RegisterProvider(topLevel);
            var user = Locator.Current.GetService<UserState>();
            ButtonsStackPanel.IsVisible = user.CurrentUser.Role == "Торговый представитель" || user.CurrentUser.Role == "Директор";
            SplitButtonPrint.IsVisible = user.CurrentUser.Role == "Оператор склада" || user.CurrentUser.Role == "Директор";
            PanelButtonCommentary.IsVisible = user.CurrentUser.Role == "Торговый представитель" || user.CurrentUser.Role == "Директор";
            TextBoxCommentary.IsEnabled = user.CurrentUser.Role == "Торговый представитель" || user.CurrentUser.Role == "Директор";
        }
        private bool _mouseDownForWindowMoving = false;
        private PointerPoint _originalPoint;

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
    }
}
