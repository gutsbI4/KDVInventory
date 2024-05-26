using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace desktop.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IUserRepository _userRepository;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuthorizationRepository _authorizationRepository;
    private readonly IViewNavigation _viewNavigation;
    private readonly IDialogService _dialogService;

    private readonly ObservableAsPropertyHelper<bool> _isUserInfoLoading;
    private readonly ObservableAsPropertyHelper<User> _user;
    private List<MenuItem> _menu;
    private MenuItem _selectedMenuItem;
    private readonly Lazy<ProductsViewModel> _productsViewModel;
    private readonly Lazy<UsersViewModel> _usersViewModel;
    private readonly Lazy<ReceiptOrderViewModel> _receiptOrderViewModel;
    private readonly Lazy<ExpenseOrdersViewModel> _expenseOrdersViewModel;
    private readonly Lazy<OrdersViewModel> _ordersViewModel;
    private ViewModelBase _selectedViewModel;
    private bool _isPaneOpen = true;
    private readonly UserState _userState;

    public MainViewModel(INotificationService notificationService, IUserRepository userRepository,
        IAuthorizationRepository authorizationRepository, IAccessTokenRepository accessTokenRepository,
        IUpdateTokenService updateTokenService, IRefreshTokenRepository refreshTokenRepository,
        IViewNavigation viewNavigation, IProductRepository productRepository, IFilePickerService filePickerService,
        ICategoryRepository categoryRepository, IDialogService dialogService, IImageService imageService,
        IFilterRepository filterRepository, IManufacturerRepository manufacturerRepository,
        IPriceUnitRepository priceUnitRepository, IBrandRepository brandRepository, IDietRepository dietRepository,
        IFillingRepository fillingRepository, IPackageRepository packageRepository,
        ITasteRepository tasteRepository, ITypeRepository typeRepository, IReceiptOrderRepository receiptOrderRepository,
        IExpenseOrderRepository expenseOrderRepository, IOrderRepository orderRepository,
        IAuditRepository auditRepository,UserState userState) : base(notificationService, updateTokenService)
    {
        _userRepository = userRepository;
        _accessTokenRepository = accessTokenRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _authorizationRepository = authorizationRepository;
        _viewNavigation = viewNavigation;
        _userState = userState;

        GetUserInfoCommand = ReactiveCommand.CreateFromTask<User>(GetUserInfo);
        GetUserInfoCommand.ThrownExceptions.Subscribe(async exc => await CommandExc(exc, GetUserInfoCommand));
        GetUserInfoCommand.IsExecuting.ToProperty(this, x => x.IsUserInfoLoading, out _isUserInfoLoading);
        _user = GetUserInfoCommand.ToProperty(this, x => x.User);
        GetUserInfoCommand.Execute().Subscribe();

        _productsViewModel = new Lazy<ProductsViewModel>(() => new ProductsViewModel(productRepository, accessTokenRepository,
            notificationService, updateTokenService, categoryRepository, dialogService, viewNavigation, filterRepository));
        _usersViewModel = new Lazy<UsersViewModel>(() => new UsersViewModel(notificationService,updateTokenService,
            userRepository, accessTokenRepository, viewNavigation,dialogService,auditRepository));
        _usersViewModel.Value.Bundle = new Bundle(this);
        _receiptOrderViewModel = new Lazy<ReceiptOrderViewModel>(() => new ReceiptOrderViewModel(notificationService, updateTokenService,
            receiptOrderRepository, accessTokenRepository, viewNavigation));
        _expenseOrdersViewModel = new Lazy<ExpenseOrdersViewModel>(() => new ExpenseOrdersViewModel(notificationService, updateTokenService,
            expenseOrderRepository, accessTokenRepository, viewNavigation));
        _ordersViewModel = new Lazy<OrdersViewModel>(() => new OrdersViewModel(notificationService, updateTokenService,
            orderRepository, accessTokenRepository, viewNavigation));

        ExitCommand = ReactiveCommand.CreateFromTask(Exit);
        ExitCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, ExitCommand));

        this.WhenAnyValue(t => t.SelectedMenuItem).Where(t => t != null).Subscribe((x) =>
        {
            switch (x.Name)
            {
                case "Выйти":
                    ExitCommand.Execute().Subscribe();
                    break;
                case "Продукция":
                    SelectedViewModel = _productsViewModel.Value;
                    break;
                case "Пользователи":
                    SelectedViewModel = _usersViewModel.Value;
                    _usersViewModel.Value.RestartLoadUsers();
                    break;
                case "Прием продукции":
                    SelectedViewModel = _receiptOrderViewModel.Value;
                    break;
                case "Списание":
                    SelectedViewModel = _expenseOrdersViewModel.Value;
                    break;
                case "Заказы":
                    SelectedViewModel = _ordersViewModel.Value;
                    break;
                default:
                    break;
            }
        });
        
    }

    public ReactiveCommand<System.Reactive.Unit, User> GetUserInfoCommand { get; }
    public bool IsUserInfoLoading => _isUserInfoLoading.Value;
    public User User => _user.Value;
    public List<MenuItem> Menu
    {
        get => _menu;
        set => this.RaiseAndSetIfChanged(ref _menu, value);
    }
    public MenuItem SelectedMenuItem
    {
        get => _selectedMenuItem;
        set => this.RaiseAndSetIfChanged(ref _selectedMenuItem, value);
    }
    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> ExitCommand { get; }
    public ViewModelBase SelectedViewModel
    {
        get => _selectedViewModel;
        set => this.RaiseAndSetIfChanged(ref _selectedViewModel, value);
    }
    public bool IsStoreKeeper { get; set; }
    public bool IsOperator { get; set; }
    public bool IsCommRepresentive { get; set; }
    public bool IsHrOfficer { get; set; }
    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
    }

    private async Task<User> GetUserInfo()
    {
        var token = _accessTokenRepository.GetAccessToken();
        User user =  await _userRepository.GetUserInfo(token);
        _userState.CurrentUser = user;
        var menuItems = new List<MenuItem>();
        menuItems.Add(new MenuItem() { Name = "Продукция", Icon = "ShoppingOutline" });
        switch (user.Role)
        {
            case "Директор":
                menuItems.Add(new MenuItem() { Name = "Прием продукции", Icon = "PackageVariantClosedPlus" });
                menuItems.Add(new MenuItem() { Name = "Списание", Icon = "PackageVariantClosedMinus" });
                menuItems.Add(new MenuItem() { Name = "Заказы", Icon = "OrderBoolDescendingVariant" });
                menuItems.Add(new MenuItem() { Name = "Пользователи", Icon = "AccountGroup" });
                IsStoreKeeper = true;
                IsOperator = true;
                IsCommRepresentive = true;
                IsHrOfficer = true;
                break;
            case "Кладовщик":
                menuItems.Add(new MenuItem() { Name = "Прием продукции", Icon = "PackageVariantClosedPlus" });
                menuItems.Add(new MenuItem() { Name = "Списание", Icon = "PackageVariantClosedMinus" });
                IsStoreKeeper = true;
                break;
            case "Оператор склада":
                menuItems.Add(new MenuItem() { Name = "Заказы", Icon = "OrderBoolDescendingVariant" });
                IsOperator = true;
                break;
            case "Торговый представитель":
                menuItems.Add(new MenuItem() { Name = "Заказы", Icon = "OrderBoolDescendingVariant" });
                IsCommRepresentive = true;
                break;
            case "Кадровик":
                menuItems.Add(new MenuItem() { Name = "Пользователи", Icon = "AccountGroup" });
                IsHrOfficer = true;
                break;
        }
        menuItems.Add(new MenuItem() { Name = "Выйти", Icon = "close" });
        Menu = menuItems;
        SelectedMenuItem = Menu[0];
        return user;
    }
    private async Task Exit()
    {
        await _authorizationRepository.Logout(_accessTokenRepository.GetAccessToken());
        _accessTokenRepository.DeleteAccessToken();
        await _refreshTokenRepository.DeleteRefreshToken();
        _viewNavigation.GoToAndCloseOthers<LoginViewModel>();
    }
    public void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}
