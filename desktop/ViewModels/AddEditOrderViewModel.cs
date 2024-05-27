using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace desktop.ViewModels
{
    public class AddEditOrderViewModel : ViewModelBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IDialogService _dialogService;
        private readonly IViewNavigation _viewNavigation;
        private readonly IOrderRepository _orderRepository;
        private readonly IDocumentRepository _documentRepository;

        private readonly ObservableAsPropertyHelper<bool> _isLoadingProducts;
        private readonly ObservableAsPropertyHelper<IEnumerable<Product>> _products;
        private ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> _cancelCommand;
        private bool _popupSearchProductOpen;
        private OrderEdit _orderEdit;
        private Product _addedProduct;
        private bool _isEnabledPrint;

        public AddEditOrderViewModel(INotificationService notificationService, IUpdateTokenService updateTokenService,
            IProductRepository productRepository, IAccessTokenRepository accessTokenRepository,
            IDialogService dialogService, IViewNavigation viewNavigation,
            IOrderRepository orderRepository,IDocumentRepository documentRepository) : base(notificationService, updateTokenService)
        {
            _productRepository = productRepository;
            _accessTokenRepository = accessTokenRepository;
            _dialogService = dialogService;
            _viewNavigation = viewNavigation;
            _orderRepository = orderRepository;
            _documentRepository = documentRepository;

            _lazyGetOrderCommand = new Lazy<ReactiveCommand<int, System.Reactive.Unit>>(() =>
            {
                var command = ReactiveCommand.CreateFromTask<int>(GetOrder);
                command.ThrownExceptions.Subscribe(async x => CommandExc(x, command));
                return command;
            });

            LoadingProducts = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(ct => this.LoadingProductsTask(ct))
                                .TakeUntil(_cancelCommand));
            LoadingProducts.IsExecuting.ToProperty(this, x => x.IsLoadingProducts, out _isLoadingProducts);
            LoadingProducts.ThrownExceptions.Subscribe(async (x) => await CommandExc(x, LoadingProducts));
            _products = LoadingProducts
                        .Where(p => p != null)
                        .Select(x => x.Products)
                        .ToProperty(this, x => x.Products, scheduler: RxApp.MainThreadScheduler);
            _cancelCommand = ReactiveCommand.Create(() => { }, LoadingProducts.IsExecuting);

            OwnersParameters = new OwnersParameters();
            OwnersParameters.WhenAnyValue(p => p.SearchString).Subscribe(p =>
            {
                if (p != null && p.Length > 0)
                {
                    RestartLoadProducts();
                    PopupSearchProductOpen = true;
                }
                else
                {
                    PopupSearchProductOpen = false;
                }
            });

            this.WhenAnyValue(x => x.AddedProduct).Subscribe(_ => AddProductForOrder());
            RemoveProductForOrder = ReactiveCommand.Create<OrderProduct>(ExecuteRemoveProductForOrder);

            this.WhenAnyValue(x => x.Bundle).Where(x => x != null).Throttle(TimeSpan.FromSeconds(0.3)).Subscribe((x) =>
            {
                int? index = (int?)x.GetParameter("idOrder");
                if (index != null)
                {
                    _lazyGetOrderCommand.Value.Execute(index.Value);
                }
                else
                {
                    OrderEdit = new OrderEdit();
                }
            });
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (OrderEdit.OrderProduct.Count < 1)
                {
                    await _dialogService.ShowDialog("Сохранение", "Нельзя оформить заказ без продукции.", IDialogService.DialogType.Standard);
                    return;
                }
                else if (String.IsNullOrEmpty(OrderEdit.Address))
                {
                    await _dialogService.ShowDialog("Сохранение", "Укажите адрес.", IDialogService.DialogType.Standard);
                    return;
                }
                int orderId = await _orderRepository.SaveOrder(_accessTokenRepository.GetAccessToken(), OrderEdit);
                _lazyGetOrderCommand.Value.Execute(orderId);
                (Bundle.OwnerViewModel as OrdersViewModel).RestartLoadOrders();
            });
            SaveCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, SaveCommand));

        }

        public bool IsLoadingProducts => _isLoadingProducts.Value;
        public readonly Lazy<ReactiveCommand<int, System.Reactive.Unit>> _lazyGetOrderCommand;
        public IEnumerable<Product> Products => _products.Value;
        public ReactiveCommand<System.Reactive.Unit, ProductsCollection> LoadingProducts { get; private set; }
        public ReactiveCommand<OrderProduct, Unit> RemoveProductForOrder { get; }
        public OwnersParameters OwnersParameters { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SaveCommand { get; private set; }
        public bool PopupSearchProductOpen
        {
            get => _popupSearchProductOpen;
            set => this.RaiseAndSetIfChanged(ref _popupSearchProductOpen, value);
        }
        public OrderEdit OrderEdit
        {
            get => _orderEdit;
            set => this.RaiseAndSetIfChanged(ref _orderEdit, value);
        }
        public Product AddedProduct
        {
            get => _addedProduct;
            set => this.RaiseAndSetIfChanged(ref _addedProduct, value);
        }
        public bool IsEnabledPrint
        {
            get => _isEnabledPrint;
            set => this.RaiseAndSetIfChanged(ref _isEnabledPrint, value);
        }

        private async Task<ProductsCollection> LoadingProductsTask(CancellationToken ct)
        {
            var accessToken = _accessTokenRepository.GetAccessToken();
            if (ct.IsCancellationRequested) return null;
            ProductsCollection? productsCollection;
            productsCollection = await _productRepository.GetProducts(accessToken, OwnersParameters);
            if (ct.IsCancellationRequested) return null;
            var productsWithoutSameProductId = productsCollection.Products
            .Where(product => !OrderEdit.OrderProduct.Any(orderProduct => orderProduct.Product.ProductId == product.ProductId))
            .ToList();
            productsCollection.Products = productsWithoutSameProductId;
            return productsCollection;
        }
        private async Task GetOrder(int id)
        {
            var order = await _orderRepository.GetOrderEdit(_accessTokenRepository.GetAccessToken(), id);
            OrderEdit = order;
            if (OrderEdit.OrderProduct.Count > 0) IsEnabledPrint = true;
            else IsEnabledPrint = false;
        }
        private void RestartLoadProducts()
        {
            _cancelCommand.Execute().Subscribe();
            LoadingProducts.Execute().Subscribe();
        }
        private void AddProductForOrder()
        {
            if (AddedProduct != null)
            {
                OrderEdit.OrderProduct.Add(new OrderProduct
                {
                    Product = AddedProduct,
                    Quantity = 1,
                    Price = AddedProduct.PriceOfSale
                });
                OwnersParameters.SearchString = "";
            }
        }
        private void ExecuteRemoveProductForOrder(OrderProduct orderProduct)
        {
            if (orderProduct != null)
            {
                OrderEdit.OrderProduct.Remove(orderProduct);
            }
        }
        public async void ShipmentOrder()
        {
            if (OrderEdit.OrderProduct.Count < 1)
            {
                await _dialogService.ShowDialog("Сохранение", "Нельзя оформить заказ без продукции.", IDialogService.DialogType.Standard);
                return;
            }
            else if (String.IsNullOrEmpty(OrderEdit.Address))
            {
                await _dialogService.ShowDialog("Сохранение", "Укажите адрес.", IDialogService.DialogType.Standard);
                return;
            }
            OrderEdit.IsShipment = true;
            await SaveCommand.Execute();
        }
        public void CancelShipOrder()
        {
            OrderEdit.IsShipment = false;
            SaveCommand.Execute();
        }
        public void Close()
        {
            _viewNavigation.Close(this);
        }
        public async Task PrintBill()
        {
            await _documentRepository.GenerateBill(OrderEdit);
            await _dialogService.ShowDialog("Печать", "Счет создан", IDialogService.DialogType.Standard);
        }
        public async Task PrintCheck()
        {
            await _documentRepository.GenerateCheck(OrderEdit);
            await _dialogService.ShowDialog("Печать", "Товарный чек создан", IDialogService.DialogType.Standard);
        }
        public async Task PrintNakladnaya()
        {
            await _documentRepository.GenerateNakladnaya(OrderEdit);
            await _dialogService.ShowDialog("Печать", "Накладная создана",IDialogService.DialogType.Standard);
        }
        public async Task PrintListKomplekt()
        {
            await _documentRepository.GenerateListKomplekt(OrderEdit);
        }
    }
}
