using Avalonia.Controls;
using Avalonia.Threading;
using desktop.Context;
using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using desktop.ViewModels;
using Refit;
using Splat;
using System;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace desktop.Views
{
    public partial class SplashWindow : Window
    {
        public const string RestApiURL= "http://localhost:5037";
        public SplashWindow()
        {
            InitializeComponent();
            Dispatcher.UIThread.Post(() => LoadApp(),DispatcherPriority.Background);
        }
        private async Task LoadApp()
        {
            var statusTextBlock = this.FindControl<TextBlock>("StatusTextBlock");
            statusTextBlock.Text = "Регистрация сервисов...";
            await Task.Run(() =>
            {
                Locator.CurrentMutable.RegisterLazySingleton<UserState>(() => new UserState());
                Locator.CurrentMutable.RegisterLazySingleton<SqLiteContext>(() => new SqLiteContext());
                Locator.CurrentMutable.RegisterLazySingleton<HttpClient>(() => RestService.CreateHttpClient(RestApiURL, null));
                Locator.CurrentMutable.Register<IUserRepository>(() => RestService.For<IUserRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.RegisterLazySingleton(() => RestService.For<IAuthorizationRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register<IProductRepository>(() => RestService.For<IProductRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register<ICategoryRepository>(() => RestService.For<ICategoryRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IManufacturerRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IBrandRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IDietRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IFillingRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IPackageRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IPriceUnitRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<ITasteRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<ITypeRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IReceiptOrderRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IExpenseOrderRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IOrderRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register(() => RestService.For<IAuditRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register<IFilterRepository>(() => RestService.For<IFilterRepository>(Locator.Current.GetService<HttpClient>()));
                Locator.CurrentMutable.Register<IImageService>(()=> RestService.For<IImageService>(Locator.Current.GetService<HttpClient>()));
                SplatRegistrations.RegisterLazySingleton<IAccessTokenRepository, AccessTokenRepository>();
                SplatRegistrations.RegisterLazySingleton<IFilePickerService, FilePickerService>();
                SplatRegistrations.RegisterLazySingleton<IDocumentRepository, DocumentService>();
                Locator.CurrentMutable.Register<IDialogService>(()=>new DesktopDialogService());
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    SplatRegistrations.RegisterLazySingleton<IRefreshTokenRepository, RefreshTokenWindowsRepository>();
                }
                else throw new PlatformNotSupportedException();
                SplatRegistrations.Register<LoginViewModel>();
                SplatRegistrations.Register<MainViewModel>();
                SplatRegistrations.Register<AddEditProductViewModel>();
                SplatRegistrations.Register<AddNewNameForEntityViewModel>();
                SplatRegistrations.Register<AddUserViewModel>();
                SplatRegistrations.Register<AddEditReceiptOrderViewModel>();
                SplatRegistrations.Register<AddEditExpenseOrderViewModel>();
                SplatRegistrations.Register<AddEditOrderViewModel>();
                SplatRegistrations.RegisterLazySingleton<IViewNavigation, WindowsNavigation>();
                SplatRegistrations.RegisterLazySingleton<INotificationService, NotificationService>();
                SplatRegistrations.RegisterLazySingleton<IUpdateTokenService, UpdateTokenService>();
                SplatRegistrations.SetupIOC();
            });

            statusTextBlock.Text = "Попытка авторизации через токен...";
            string? token = await Task<string?>.Run(() =>
            {
                IRefreshTokenRepository refreshTokenRepository;
                refreshTokenRepository = Locator.Current.GetService<IRefreshTokenRepository>();
                return refreshTokenRepository.GetRefreshToken();
            });
            if(token == null)
            {
                Locator.Current.GetService<IViewNavigation>().GoToAndCloseCurrent<LoginViewModel>((ViewModelBase)DataContext);
                return;
            }
            try
            {
                var newTokens = await Locator.Current.GetService<IAuthorizationRepository>().UpdateToken(token);
                await Locator.Current.GetService<IRefreshTokenRepository>().UpdateRefreshToken(newTokens.RefreshToken);
                Locator.Current.GetService<IAccessTokenRepository>().AddAccessToken(newTokens.AccessToken);
                statusTextBlock.Text = "Входим в систему...";
                Locator.Current.GetService<IViewNavigation>().GoToAndCloseCurrent<MainViewModel>((ViewModelBase)DataContext);
            }
            catch (HttpRequestException)
            {
                statusTextBlock.Text = "Не удалось подключиться к серверу, выходим...";
                await Task.Delay(2000);
                this.Close();
                return;
            }
            catch(ApiException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    statusTextBlock.Text = "Ошибка со стороны сервера, выходим...";
                    await Task.Delay(2000);
                    this.Close();
                    return;
                }
                await Locator.Current.GetService<IRefreshTokenRepository>().DeleteRefreshToken();
                Locator.Current.GetService<IViewNavigation>().GoToAndCloseCurrent<LoginViewModel>((ViewModelBase)DataContext);
            }
        }
    }
}
