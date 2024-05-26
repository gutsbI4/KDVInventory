using Avalonia.Controls;
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
    }
}
