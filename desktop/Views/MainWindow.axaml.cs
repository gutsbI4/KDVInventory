using Avalonia.Controls;
using desktop.Services;
using Splat;

namespace desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var topLevel = TopLevel.GetTopLevel(this);
        Locator.Current.GetService<IFilePickerService>().RegisterProvider(topLevel);
    }
}