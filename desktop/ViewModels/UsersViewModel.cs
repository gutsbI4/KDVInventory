using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace desktop.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IViewNavigation _viewNavigation;
        private readonly IDialogService _dialogService;
        private readonly IAuditRepository _auditRepository;


        private readonly ObservableAsPropertyHelper<bool> _isLoadingUsers;
        private readonly ObservableAsPropertyHelper<bool> _isLoadingAudits;
        private readonly ObservableAsPropertyHelper<IEnumerable<User>> _users;
        private readonly ObservableAsPropertyHelper<IEnumerable<Audit>> _audit;
        private ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> _cancelCommand;
        private User _selectedUser;
        private bool _isPaneOpen;
        public UsersViewModel(INotificationService notificationService, IUpdateTokenService updateTokenService,
            IUserRepository userRepository, IAccessTokenRepository accessTokenRepository,
            IViewNavigation viewNavigation,IDialogService dialogService,
            IAuditRepository auditRepository) : base(notificationService, updateTokenService)
        {
            _accessTokenRepository = accessTokenRepository;
            _userRepository = userRepository;
            _viewNavigation = viewNavigation;
            _dialogService = dialogService;
            _auditRepository = auditRepository;

            LoadingUsers = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(ct => this.LoadingUsersTask(ct))
                                .TakeUntil(_cancelCommand));
            LoadingUsers.IsExecuting.ToProperty(this, x => x.IsLoadingUsers, out _isLoadingUsers);
            LoadingUsers.ThrownExceptions.Subscribe(async (x) => await CommandExc(x, LoadingUsers));
            _users = LoadingUsers
                        .Where(p => p != null)
                        .ToProperty(this, x => x.Users, scheduler: RxApp.MainThreadScheduler);
            _cancelCommand = ReactiveCommand.Create(() => { }, LoadingUsers.IsExecuting);

            LoadingAudits = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(ct => this.LoadingAuditsTask(ct))
                                .TakeUntil(_cancelCommand));
            LoadingAudits.IsExecuting.ToProperty(this, x => x.IsLoadingAudits, out _isLoadingAudits);
            LoadingAudits.ThrownExceptions.Subscribe(async (x) => await CommandExc(x, LoadingAudits));
            _audit = LoadingAudits
                        .Where(p => p != null)
                        .ToProperty(this, x => x.Audits, scheduler: RxApp.MainThreadScheduler);
            _cancelCommand = ReactiveCommand.Create(() => { }, LoadingAudits.IsExecuting);

            LoadingUsers.Execute().Subscribe();

            this.WhenAnyValue(x => x.IsArchiveSelected).Subscribe(_=> RestartLoadUsers());

            EditUserCommand = ReactiveCommand.Create<int>(EditUser);
            ArchiveUser = ReactiveCommand.CreateFromTask<User>(ExecuteArchiveUser);
            NotArchiveUser = ReactiveCommand.CreateFromTask<User>(ExecuteNotArchiveUser);

            DownloadAuditsCommand = ReactiveCommand.CreateFromTask<DateTime>(DownloadAudits);
        }

        private bool _isArchiveSelected = false;

        public bool IsLoadingUsers => _isLoadingUsers.Value;
        public bool IsLoadingAudits => _isLoadingAudits.Value;
        public ReactiveCommand<System.Reactive.Unit, IEnumerable<User>> LoadingUsers { get; private set; }
        public ReactiveCommand<System.Reactive.Unit, IEnumerable<Audit>> LoadingAudits { get; private set; }
        public ReactiveCommand<User, Unit> ArchiveUser { get; }
        public ReactiveCommand<User, Unit> NotArchiveUser { get; }
        public ReactiveCommand<DateTime, Unit> DownloadAuditsCommand { get; }
        public IEnumerable<User> Users => _users.Value;
        public IEnumerable<Audit> Audits => _audit.Value;
        public bool IsArchiveSelected
        {
            get => _isArchiveSelected;
            set => this.RaiseAndSetIfChanged(ref _isArchiveSelected, value);
        }
        public User SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
        }
        public ReactiveCommand<int, System.Reactive.Unit> EditUserCommand { get; }


        private async Task<IEnumerable<User>> LoadingUsersTask(CancellationToken ct)
        {
            var accessToken = _accessTokenRepository.GetAccessToken();
            if (ct.IsCancellationRequested) return null;
            IEnumerable<User>? usersCollection;
            usersCollection = await _userRepository.GetUsers(accessToken);
            if (ct.IsCancellationRequested) return null;
            return usersCollection.Where(x=>x.IsArchive == IsArchiveSelected);
        }
        private async Task<IEnumerable<Audit>> LoadingAuditsTask(CancellationToken ct)
        {
            var accessToken = _accessTokenRepository.GetAccessToken();
            if (ct.IsCancellationRequested) return null;

            IsPaneOpen = true;

            IEnumerable<Audit>? auditsCollection;
            auditsCollection = await _auditRepository.GetAuditByUserId(accessToken, SelectedUser.IdUser);
            if (ct.IsCancellationRequested)
            {
                IsPaneOpen = false;
                return null;
            }

            return auditsCollection;
        }
        public void RestartLoadUsers()
        {
            _cancelCommand.Execute().Subscribe();
            LoadingUsers.Execute().Subscribe();
        }
        public async Task RestartLoadAuditsAsync()
        {
            _cancelCommand?.Execute();
            await LoadingAudits.Execute();
        }
        public void AddUser()
        {
            Bundle bundle = new Bundle(this);
            _viewNavigation.GoTo<AddUserViewModel>(bundle);
        }
        private void EditUser(int id)
        {
            Bundle bundle = new Bundle(this);
            bundle.AddNewParameter("idUser", id);
            _viewNavigation.GoTo<AddUserViewModel>(bundle);
        }
        private async Task ExecuteArchiveUser(User user)
        {
            var vm = Bundle.OwnerViewModel as MainViewModel;
            if (vm != null && vm.User.IdUser == user.IdUser)
            {
                var resultM = await _dialogService.ShowDialog("Внимание",
                    "Вы уверены, что хотите архивировать себя? У вас не будет доступа к системе!",
                    IDialogService.DialogType.YesNoDialog);
                if (resultM == IDialogService.DialogResult.Yes)
                {
                    var userEdit = await _userRepository.GetUserEdit(_accessTokenRepository.GetAccessToken(), user.IdUser);
                    if (userEdit != null)
                    {
                        userEdit.IsArchive = true;
                        await _userRepository.UpdateUser(_accessTokenRepository.GetAccessToken(), userEdit);
                        RestartLoadUsers();
                    }
                    _viewNavigation.GoToAndCloseOthers<LoginViewModel>();
                }
                return;
            }
            var result = await _dialogService.ShowDialog("Внимание",
                    "Вы уверены, что хотите архивировать пользователя?",
                    IDialogService.DialogType.YesNoDialog);
            if (result == IDialogService.DialogResult.Yes)
            {
                var userEdit = await _userRepository.GetUserEdit(_accessTokenRepository.GetAccessToken(), user.IdUser);
                if (userEdit != null)
                {
                    userEdit.IsArchive = true;
                    await _userRepository.UpdateUser(_accessTokenRepository.GetAccessToken(), userEdit);
                    RestartLoadUsers();
                }
            }
            else return;
            
        }
        private async Task ExecuteNotArchiveUser(User user)
        {
            
            var result = await _dialogService.ShowDialog("Внимание",
                    "Вы уверены, что хотите вернуть пользователя из архива?",
                    IDialogService.DialogType.YesNoDialog);
            if (result == IDialogService.DialogResult.Yes)
            {
                var userEdit = await _userRepository.GetUserEdit(_accessTokenRepository.GetAccessToken(), user.IdUser);
                if (userEdit != null)
                {
                    userEdit.IsArchive = false;
                    await _userRepository.UpdateUser(_accessTokenRepository.GetAccessToken(), userEdit);
                    RestartLoadUsers();
                }
            }
            else return;
        }
        private async Task DownloadAudits(DateTime date)
        {
            try
            {
                await _auditRepository.SaveAuditToFile(_accessTokenRepository.GetAccessToken(), SelectedUser.IdUser,
                new AuditTime { Date = date });
                await _dialogService.ShowDialog("Аудит", "Аудит успешно скачен в папку AuditReports на сервере.", IDialogService.DialogType.Standard);

            }
            catch (Exception)
            {
                await _dialogService.ShowDialog("Ошибка", "Не получилось скачать аудит.", IDialogService.DialogType.Standard);
            }
             


        }
    }
}
