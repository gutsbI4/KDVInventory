using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace desktop.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IViewNavigation _viewNavigation;



        private readonly ObservableAsPropertyHelper<bool> _isLoadingOrders;
        private readonly ObservableAsPropertyHelper<IEnumerable<Order>> _orders;
        private ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> _cancelCommand;
        private readonly ObservableAsPropertyHelper<int> _countPage; 
        private readonly ObservableAsPropertyHelper<int> _countOrders;
        private bool _isOrderSelected = false;

        public OrdersViewModel(INotificationService notificationService, IUpdateTokenService updateTokenService,
            IOrderRepository orderRepository, IAccessTokenRepository accessTokenRepository,
            IViewNavigation viewNavigation) : base(notificationService, updateTokenService)
        {
            _accessTokenRepository = accessTokenRepository;
            _orderRepository = orderRepository;
            _viewNavigation = viewNavigation;

            LoadingOrders = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(ct => this.LoadingOrdersTask(ct))
                                .TakeUntil(_cancelCommand));
            LoadingOrders.IsExecuting.ToProperty(this, x => x.IsLoadingOrders, out _isLoadingOrders);
            LoadingOrders.ThrownExceptions.Subscribe(async (x) => await CommandExc(x, LoadingOrders));
            _orders = LoadingOrders
                        .Where(p => p != null).Select(x=>x.Orders)
                        .ToProperty(this, x => x.Orders, scheduler: RxApp.MainThreadScheduler);
            _cancelCommand = ReactiveCommand.Create(() => { }, LoadingOrders.IsExecuting);
            
            _countOrders = LoadingOrders.Where(x => x != null).Select(x => x.Count).ToProperty(this, x => x.CountOrders, scheduler: RxApp.MainThreadScheduler);
            

            OwnersParameters = new OwnersParameters(); 
            this.WhenAnyValue(x => x.CountOrders, x => x.OwnersParameters.SizePage)
                .Where(x => x.Item2 != 0)
                .Select(x => (x.Item1 + x.Item2 - 1) / x.Item2)
                .ToProperty(this, x => x.CountPage, out _countPage);
            OwnersParameters.WhenAnyValue(p => p.PageNumber).Subscribe(_ => RestartLoadOrders());
            OwnersParameters.WhenAnyValue(p => p.SizePage).Subscribe(_ => GoToFirstPageAndRestartLoadOrders());
            OwnersParameters.WhenAnyValue(p => p.SearchString)
                            .Throttle(TimeSpan.FromSeconds(0.75)).Subscribe(_ => GoToFirstPageAndRestartLoadOrders());

            LoadingOrders.Execute().Subscribe();

            EditOrderCommand = ReactiveCommand.Create<int>(EditOrder);
            this.WhenAnyValue(x => x.IsShipmentSelected).Subscribe(_ => GoToFirstPageAndRestartLoadOrders()); //
            DateRange = new DateRange();
            DateRange.WhenAnyValue(x => x.DateOne, x => x.DateTwo).Subscribe(_ => GoToFirstPageAndRestartLoadOrders()); //

        }

        public bool IsLoadingOrders => _isLoadingOrders.Value;
        public ReactiveCommand<System.Reactive.Unit, OrderCollection> LoadingOrders { get; private set; }
        public IEnumerable<Order> Orders => _orders.Value;
        public ReactiveCommand<int, System.Reactive.Unit> EditOrderCommand { get; }
        public int CountPage => _countPage.Value;
        public int CountOrders => _countOrders.Value; 
        public IEnumerable<int> PageCounts => new int[] { 10, 20, 50, 100 }; 
        public OwnersParameters OwnersParameters { get; set; }
        public DateRange DateRange { get; set; } //
        public bool IsShipmentSelected //
        {
            get => _isOrderSelected;
            set => this.RaiseAndSetIfChanged(ref _isOrderSelected, value);
        }

        private async Task<OrderCollection> LoadingOrdersTask(CancellationToken ct) 
        {
            var accessToken = _accessTokenRepository.GetAccessToken();
            if (ct.IsCancellationRequested) return null;
            var ordersCollection = await _orderRepository.GetOrders(accessToken,OwnersParameters);
            if (ct.IsCancellationRequested) return null;
            if (IsShipmentSelected) //
            {
                ordersCollection.Orders = ordersCollection.Orders.Where(x => x.IsShipment == IsShipmentSelected);
            }
            if (DateRange.DateOne != null && DateRange.DateTwo != null) //
                ordersCollection.Orders = ordersCollection.Orders
                    .Where(x => x.DateOfOrder >= DateRange.DateOne && x.DateOfOrder <= DateRange.DateTwo);
            return ordersCollection;
        }
        public void RestartLoadOrders()
        {
            _cancelCommand.Execute().Subscribe();
            LoadingOrders.Execute().Subscribe();
        }
        public void AddOrder()
        {
            Bundle bundle = new Bundle(this);
            _viewNavigation.GoTo<AddEditOrderViewModel>(bundle);
        }
        private void EditOrder(int id)
        {
            Bundle bundle = new Bundle(this);
            bundle.AddNewParameter("idOrder", id);
            _viewNavigation.GoTo<AddEditOrderViewModel>(bundle);
        }
        public void NextPage()
        {
            if (OwnersParameters.PageNumber < CountPage) OwnersParameters.PageNumber++;
        }
        public void PreviousPage()
        {
            if (OwnersParameters.PageNumber > 1)
                OwnersParameters.PageNumber--;
        }
        public void GoFirstPage() => OwnersParameters.PageNumber = 1;
        public void GoLastPage() => OwnersParameters.PageNumber = CountPage; 
        public void GoToFirstPageAndRestartLoadOrders()
        {
            if (OwnersParameters.PageNumber == 1)
                RestartLoadOrders();
            else OwnersParameters.PageNumber = 1;
        }
        public void CancelDateSearch() //
        {
            DateRange.DateOne = null;
            DateRange.DateTwo = null;
        }
    }
}
