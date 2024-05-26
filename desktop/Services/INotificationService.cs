using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services
{
    public interface INotificationService
    {
        public void RegisterNotificationManager(INotificationManager notificationManager);
        public void ShowNotification(INotification notification);
    }
}
