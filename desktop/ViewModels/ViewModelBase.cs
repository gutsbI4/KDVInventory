using Avalonia.Controls.Notifications;
using desktop.Models;
using desktop.Services;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Helpers;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace desktop.ViewModels;

public class ViewModelBase : ReactiveValidationObject
{
    protected readonly INotificationService _notificationService;
    protected readonly IUpdateTokenService _updateTokenService;
    protected readonly ValidationContext _validationContext = new ValidationContext();

    private Bundle _bundle;
    private string _title;
    public ViewModelBase(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public ViewModelBase(INotificationService notificationService, IUpdateTokenService updateTokenService)
    {
        _notificationService = notificationService;
        _updateTokenService = updateTokenService;
    }
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
    public Bundle Bundle
    {
        get => _bundle;
        set => this.RaiseAndSetIfChanged(ref _bundle, value);
    }

    public ValidationContext ValidationContext => _validationContext;

    public async Task CommandExc(Exception exp, object command)
    {
        switch (exp)
        {
            case HttpRequestException:
                _notificationService.ShowNotification(new Notification("", "Не удалось соединить соединение с сервером", NotificationType.Error));
                break;
            case ApiException ex:
                _notificationService.ShowNotification(new Notification("", ex.Content, NotificationType.Error));
                break;
            case Exception:
                _notificationService.ShowNotification(new Notification("Ошибка", "Неизвестная ошибка", NotificationType.Error));
                break;
        }
    }

}
