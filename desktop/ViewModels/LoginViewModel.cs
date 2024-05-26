using Avalonia.Controls.Notifications;
using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using ReactiveUI;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace desktop.ViewModels
{
    public class LoginViewModel: ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<bool> _isLogin;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IViewNavigation _viewNavigation;
        public LoginViewModel(IAccessTokenRepository accessTokenRepository, IRefreshTokenRepository refreshTokenRepository, IAuthorizationRepository authorizationRepository,
            INotificationService notificationService, IViewNavigation viewNavigation) : base(notificationService)
        {
            _accessTokenRepository = accessTokenRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _authorizationRepository = authorizationRepository;
            _viewNavigation = viewNavigation;

            Login = ReactiveCommand.CreateFromTask(LoginTask);
            Login.IsExecuting.ToProperty(this, x => x.IsLogin, out _isLogin);
            Login.ThrownExceptions.Subscribe(async (x) => await CommandExc(x, LoginTask));
            AuthorizationData = new Authorization();
        }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Login { get; private set; }
        public Authorization AuthorizationData { get; private set; }
        public bool IsLogin => _isLogin.Value;
        public async Task LoginTask()
        {
            if(string.IsNullOrEmpty(AuthorizationData.Login) && string.IsNullOrEmpty(AuthorizationData.Password))
            {
                _notificationService.ShowNotification(new Avalonia.Controls.Notifications.Notification("Ошибка", "Заполните поля формы", NotificationType.Error));
                return;
            }
            Token token = await _authorizationRepository.Login(AuthorizationData);
            await _refreshTokenRepository.AddRefreshToken(token.RefreshToken);
            _accessTokenRepository.AddAccessToken(token.AccessToken);
            _viewNavigation.GoToAndCloseCurrent<MainViewModel>(this);
        }
    }
}
