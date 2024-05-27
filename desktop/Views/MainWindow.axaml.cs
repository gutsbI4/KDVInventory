using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using desktop.Services;
using desktop.ViewModels;
using Splat;
using System.Linq;

namespace desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Closed(object? sender, System.EventArgs e)
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
        foreach (var window in lifetime.Windows.ToList())
        {
            if (window.DataContext is LoginViewModel) return;
            window.Close();
        }
    }
}