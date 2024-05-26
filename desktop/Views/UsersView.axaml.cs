using Avalonia.Controls;
using Avalonia.Interactivity;
using desktop.Models;
using desktop.ViewModels;
using System;
using System.Threading.Tasks;

namespace desktop.Views
{
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
        }


        private async void DataGrid_CellPointerPressed(object? sender, Avalonia.Controls.DataGridCellPointerPressedEventArgs e)
        {
            if (e.Cell.Content is Button) return;
            else if (e.Column.Header == "Последнее действие совершено" || e.Column.Header == "Вход сегодня")
            {
                var selectedUser = e.Row.DataContext as User;
                if (selectedUser != null)
                {
                    var usersViewModel = DataContext as UsersViewModel;
                    if (usersViewModel != null)
                    {
                        usersViewModel.SelectedUser = selectedUser;
                        await usersViewModel.RestartLoadAuditsAsync();
                    }
                }
                return;
            }
            var vm = DataContext as UsersViewModel;
            if (vm != null)
            {
                var user = e.Cell.DataContext as User;
                if (user == null) return;
                vm.EditUserCommand.Execute(user.IdUser).Subscribe();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var usersViewModel = DataContext as UsersViewModel;
            if (usersViewModel != null)
            {
                usersViewModel.IsPaneOpen = false;
            }
        }
    }
}
