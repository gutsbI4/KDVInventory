using Avalonia.Controls.Notifications;
using desktop.Models;
using desktop.Services.Repositories;
using desktop.ViewModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services
{
    public class UpdateTokenService : IUpdateTokenService
    {
        private IAuthorizationRepository _authorizationRepository;
        private IRefreshTokenRepository _refreshTokenRepository;
        private IAccessTokenRepository _accessTokenRepository;
        private INotificationService _notificationService;
        private IViewNavigation _viewNavigation;
        public UpdateTokenService(IAuthorizationRepository authorizationRepository, IRefreshTokenRepository refreshTokenRepository,
            IAccessTokenRepository accessTokenRepository, INotificationService notificationService, IViewNavigation viewNavigation)
        {
            _authorizationRepository = authorizationRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _accessTokenRepository = accessTokenRepository;
            _notificationService = notificationService;
            _viewNavigation = viewNavigation;
        }

        public async  Task UpdateTokens()
        {
            try
            {
                string refreshToken = await _refreshTokenRepository.GetRefreshToken();
                Token token = await _authorizationRepository.UpdateToken(refreshToken);
                await _refreshTokenRepository.UpdateRefreshToken(token.RefreshToken);
                _accessTokenRepository.UpdateAccessToken(token.AccessToken);
            }
            catch (HttpRequestException)
            {
                _notificationService.ShowNotification(new Notification("Ошибка сервера", "Не удалось установить подключение с сервером.", NotificationType.Error));
            }
            catch (ApiException apiException)
            {
                switch (apiException.StatusCode)
                {
                    case System.Net.HttpStatusCode.InternalServerError:
                        _notificationService.ShowNotification(new Notification("Ошибка сервера", "Не удалось установить покдлючение с сервером.", NotificationType.Error));
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                        _accessTokenRepository.DeleteAccessToken();
                        await _refreshTokenRepository.DeleteRefreshToken();
                        _viewNavigation.GoToAndCloseOthers<LoginViewModel>();
                        break;
                }
            }
        }
    }
}
