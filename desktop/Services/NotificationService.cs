using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services
{
    public class NotificationService : INotificationService
    {
        private INotificationManager? _notificationManager;
        public void RegisterNotificationManager(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        public void ShowNotification(INotification notification)
        {
            _notificationManager?.Show(notification);
        }
    }
}
