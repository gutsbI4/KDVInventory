using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using desktop.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services
{
    public class WindowsNavigation : IViewNavigation
    {
        private readonly Dictionary<Type, Window> _openWindows = new Dictionary<Type, Window>();

        public void Close(ViewModelBase viewModel)
        {
            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
            var window = lifetime.Windows.FirstOrDefault(w => w.DataContext == viewModel);
            if (window != null)
            {
                window.Close();
                var entry = _openWindows.FirstOrDefault(kvp => kvp.Value == window);
                if (!entry.Equals(default(KeyValuePair<Type, Window>)))
                {
                    _openWindows.Remove(entry.Key);
                }
            }
        }

        public void GoTo<VM>(Models.Bundle bundle = null) where VM : ViewModelBase
        {
            ShowWindow<VM>(bundle);
        }

        public void GoToAndCloseCurrent<VM>(ViewModelBase currentViewModel, Models.Bundle bundle = null) where VM : ViewModelBase
        {
            ShowWindow<VM>(bundle);
            Close(currentViewModel);
        }

        public void GoToAndCloseOthers<VM>(Models.Bundle bundle = null) where VM : ViewModelBase
        {
            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;

            ShowWindow<VM>(bundle);

            // Закроем все окна, кроме только что открытого
            foreach (var window in lifetime.Windows.ToList())
            {
                if (!(window.DataContext is VM))
                {
                    window.Close();
                }
            }

            // Очистим коллекцию, поскольку все окна кроме одного должны быть закрыты
            _openWindows.Clear();
        }

        private void ShowWindow<VM>(Models.Bundle bundle) where VM : ViewModelBase
        {
            var name = $"{Assembly.GetEntryAssembly()?.GetName().Name}.Views.{typeof(VM).Name.Replace("ViewModel", "Window")}";
            var type = Type.GetType(name);
            if (type == null)
                throw new Exception("View not found");

            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;

            // Проверка всех открытых окон, чтобы проверить, существует ли уже окно такого типа
            var existingWindow = lifetime.Windows.FirstOrDefault(w => w.GetType() == type);
            if (existingWindow != null)
            {
                existingWindow.Close();
                var entry = _openWindows.FirstOrDefault(kvp => kvp.Value == existingWindow);
                if (!entry.Equals(default(KeyValuePair<Type, Window>)))
                {
                    _openWindows.Remove(entry.Key);
                }
            }

            Window window = (Window)Activator.CreateInstance(type)!;
            window.Activated += (obj, e) => Locator.Current.GetService<INotificationService>()?
                .RegisterNotificationManager(new WindowNotificationManager(window));

            ViewModelBase viewModelBase = Locator.Current.GetService<VM>();
            viewModelBase.Bundle = bundle;
            window.DataContext = viewModelBase;

            // Регистрацию окна лучше делать после того, как оно фактически откроется и станет активным
            window.Closed += (sender, args) =>
            {
                if (_openWindows.ContainsKey(typeof(VM))) _openWindows.Remove(typeof(VM));
            };

            window.Show();
            window.Focus();

            // Добавлять в коллекцию открытых окон после вызова Show
            _openWindows[typeof(VM)] = window;
        }
    }

}
