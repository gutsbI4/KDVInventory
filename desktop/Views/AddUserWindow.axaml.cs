using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using desktop.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Disposables;

namespace desktop.Views
{
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
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
        private void ComboBox_DropDownOpened(object? sender, System.EventArgs e)
        {
            _mouseDownForWindowMoving = false;
        }

        private void ComboBox_DropDownClosed_1(object? sender, System.EventArgs e)
        {
            _mouseDownForWindowMoving = true;
        }
    }
}
