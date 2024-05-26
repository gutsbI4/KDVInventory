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
            ButtonsStackPanel.IsVisible = user.CurrentUser.Role == "�������� �������������" || user.CurrentUser.Role == "��������";
            SplitButtonPrint.IsVisible = user.CurrentUser.Role == "�������� ������" || user.CurrentUser.Role == "��������";
            PanelButtonCommentary.IsVisible = user.CurrentUser.Role == "�������� �������������" || user.CurrentUser.Role == "��������";
            TextBoxCommentary.IsEnabled = user.CurrentUser.Role == "�������� �������������" || user.CurrentUser.Role == "��������";
        }
    }
}
