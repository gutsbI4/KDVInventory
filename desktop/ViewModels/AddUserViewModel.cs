using Avalonia.Data.Core.Plugins;
using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using desktop.Views;
using DocumentFormat.OpenXml.Wordprocessing;
using MsBox.Avalonia;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using Avalonia.Controls.Notifications;
using Refit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Notification = Avalonia.Controls.Notifications.Notification;
using Splat;
using System.Text.RegularExpressions;

namespace desktop.ViewModels
{
    public class AddUserViewModel : ViewModelBase
    {
        private readonly IViewNavigation _viewNavigation;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IDialogService _dialogService;
        private readonly IUserRepository _userRepository;

        public AddUserViewModel(INotificationService notificationService, IUpdateTokenService updateTokenService,
            IViewNavigation viewNavigation, IAccessTokenRepository accessTokenRepository,
            IDialogService dialogService, IUserRepository userRepository) : base(notificationService, updateTokenService)
        {
            _viewNavigation = viewNavigation;
            _accessTokenRepository = accessTokenRepository;
            _dialogService = dialogService;
            _userRepository = userRepository;

            IObservable<bool> passwordsAddObservable =
                this.WhenAnyValue(
                    x => x.User.Password,
                    x => x.ConfirmPasswordAdd,
                    (password, confirmation) => password == confirmation);

            this.ValidationRule(
                vm => vm.ConfirmPasswordAdd,
                passwordsAddObservable,
                "Пароли должны совпадать.");

            _lazyGetUserCommand = new Lazy<ReactiveCommand<int, System.Reactive.Unit>>(() =>
            {
                var command = ReactiveCommand.CreateFromTask<int>(GetUser);
                command.ThrownExceptions.Subscribe(async x => CommandExc(x, command));
                return command;
            });
            GetRolesCommand = ReactiveCommand.CreateFromTask(async () => await _userRepository.GetRoles(_accessTokenRepository.GetAccessToken()));
            GetRolesCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetRolesCommand));
            _roles = GetRolesCommand.ToProperty(this, x => x.Roles);
            GetRolesCommand.Execute().Subscribe();


            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                
                if (Bundle.GetParameter("idUser") != null)
                    await _userRepository.UpdateUser(_accessTokenRepository.GetAccessToken(), User);
                else
                {
                    if (User.Password != ConfirmPasswordAdd)
                    {
                        await _dialogService.ShowDialog("Сохранение пользователя",
                        "Пароль и подтверждение пароля должны совпадать", IDialogService.DialogType.Standard);
                        return;
                    }
                    else if (User.Password.Length < 7)
                    {
                        await _dialogService.ShowDialog("Сохранение пользователя",
                        "Пароль должен содержать как минимум 7 символов.", IDialogService.DialogType.Standard);
                        return;
                    }
                    await _userRepository.AddUser(_accessTokenRepository.GetAccessToken(), User);
                }   
                (Bundle?.OwnerViewModel as UsersViewModel)?.RestartLoadUsers();
                _viewNavigation.Close(this);
            });
            SaveCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, SaveCommand));

            

            ChangePasswordCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var response = await _userRepository.ChangePassword(_accessTokenRepository.GetAccessToken(), ChangePassword);
                if (response.IsSuccessStatusCode)
                {
                    CloseChangePassword();
                    await _dialogService.ShowDialog("Смена пароля",
                    "Пароль успешно сменен!", IDialogService.DialogType.Standard);
                }
                else
                {
                    _notificationService.ShowNotification(new Notification("Смена пароля", response.Error.Content,NotificationType.Error));
                }
                
            }, canExecute: this.IsValid());
            ChangePasswordCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, ChangePasswordCommand));
            _isUserModified = false;
            Observable.Merge(
                    this.WhenAnyValue(x => x.User.Login).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.User.Password).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.User.Image).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x =>x.User.RoleId).Skip(2).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.User.IsActive).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.User.IsArchive).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.User.Employee.Surname).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.User.Employee.Name).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.User.Employee.MiddleName).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.User.Employee.PhoneNumber).Skip(1).Select(_ => Unit.Default)
                ).Subscribe(_ =>
                {
                    _isUserModified = true;
                });
            this.WhenAnyValue(x => x.Bundle).Where(x => x != null).Throttle(TimeSpan.FromSeconds(0.3)).Subscribe((x) =>
            {
                int? index = (int?)x.GetParameter("idUser");
                if (index != null)
                {
                    Title = "Редактировать сотрудника";
                    ButtonSaveText = "Сохранить";
                    _lazyGetUserCommand.Value.Execute(index.Value);
                    IsAddUser = false;
                }
                else
                {
                    Title = "Добавить нового сотрудника";
                    ButtonSaveText = "Добавить";
                    User = new UserEdit();
                    User.Employee = new Employee();
                    IsAddUser = true;
                    ButtonActiveText = "Отключить доступ";
                    TextBlockActiveText = "Статус: Активен";
                    User.RoleId = Roles.Select(p => p.RoleId).FirstOrDefault();
                }
            });
            
            

        }

        private UserEdit _user;
        private ChangePassword _changePassword;
        private string _confirmPassword;
        private string _confirmPasswordAdd = "";
        public readonly Lazy<ReactiveCommand<int, System.Reactive.Unit>> _lazyGetUserCommand;
        private readonly ObservableAsPropertyHelper<IEnumerable<Role>> _roles;
        private string _buttonSaveText;
        private string _buttonActiveText;
        private string _textBlockActiveText;
        private bool _isUserModified = false;
        private bool _popupChangePassOpen;
        private bool _isAddUser;

        public IEnumerable<Role> Roles => _roles.Value;
        public ReactiveCommand<System.Reactive.Unit, IEnumerable<Role>> GetRolesCommand { get; private set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SaveCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ChangePasswordCommand { get; private set; }
        public UserEdit User
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }
        public bool IsAddUser
        {
            get => _isAddUser;
            set => this.RaiseAndSetIfChanged(ref _isAddUser, value);
        }
        public ChangePassword ChangePassword
        {
            get => _changePassword;
            set => this.RaiseAndSetIfChanged(ref _changePassword, value);
        }
        public string ButtonSaveText
        {
            get => _buttonSaveText;
            set => this.RaiseAndSetIfChanged(ref _buttonSaveText, value);
        }
        public string ButtonActiveText
        {
            get => _buttonActiveText;
            set => this.RaiseAndSetIfChanged(ref _buttonActiveText, value);
        }
        public string TextBlockActiveText
        {
            get => _textBlockActiveText;
            set => this.RaiseAndSetIfChanged(ref _textBlockActiveText, value);
        }
        public bool PopupChangePassOpen
        {
            get => _popupChangePassOpen;
            set => this.RaiseAndSetIfChanged(ref _popupChangePassOpen, value);
        }
        
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
        }
        public string ConfirmPasswordAdd
        {
            get => _confirmPassword;
            set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
        }
        private async Task GetUser(int id)
        {
            var user = await _userRepository.GetUserEdit(_accessTokenRepository.GetAccessToken(), id);
            User = user;
            if (User.Employee == null) User.Employee = new Employee();
            if (User.IsActive)
            {
                ButtonActiveText = "Отключить доступ";
                TextBlockActiveText = "Статус: Активен";
            }
            else
            { 
                ButtonActiveText = "Включить доступ";
                TextBlockActiveText = "Статус: Не активен";
            }
        }
        public async void Exit()
        {
            if (_isUserModified)
            {
                var result = await _dialogService.ShowDialog("Внимание",
                    "На странице имеются несохранённые данные. Вы хотите уйти со страницы без сохранения изменений?",
                    IDialogService.DialogType.YesNoDialog);
                if (result == IDialogService.DialogResult.Yes) _viewNavigation.Close(this);
                else return;
            }
            _viewNavigation.Close(this);
        }
        public void ClickOnActiveButton()
        {
            if (ButtonActiveText == "Отключить доступ")
            {
                User.IsActive = false;
                ButtonActiveText = "Включить доступ";
                TextBlockActiveText = "Статус: Не активен";

            }
            else
            {
                User.IsActive = true;
                ButtonActiveText = "Отключить доступ";
                TextBlockActiveText = "Статус: Активен";
            }
        }
        public void ClickOnChangePassword()
        {
            IObservable<bool> passwordsObservable =
                this.WhenAnyValue(
                    x => x.ChangePassword.NewPassword,
                    x => x.ConfirmPassword,
                    (password, confirmation) => password == confirmation);

            this.ValidationRule(
                vm => vm.ConfirmPassword,
                passwordsObservable,
                "Пароли должны совпадать.");
            this.ValidationRule(
            viewModel => viewModel.ChangePassword.OldPassword,
            name => !string.IsNullOrWhiteSpace(name),
            "Пароль обязателен к заполнению.");
            this.ValidationRule(
            vm => vm.ChangePassword.NewPassword,
                pass => !string.IsNullOrWhiteSpace(pass) && pass.Length > 6 && pass.Length < 20,
                "Пароль обязателен к заполнению. Длина должна быть больше 6 символов и меньше 20.");
            this.ClearValidationRules(vm => vm.ConfirmPasswordAdd);
            PopupChangePassOpen = true;
            ChangePassword = new ChangePassword() { UserId = User.UserId};
            
        }
        public void CloseChangePassword()
        {
            PopupChangePassOpen = false;
            ConfirmPassword = "";
            
        }
    }
}
