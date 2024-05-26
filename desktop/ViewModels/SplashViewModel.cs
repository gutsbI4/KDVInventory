using desktop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        public SplashViewModel(INotificationService notificationService) : base(notificationService)
        {
        }
    }
}
