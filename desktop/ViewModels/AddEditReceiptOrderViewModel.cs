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
    public class AddEditReceiptOrderViewModel : ViewModelBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IDialogService _dialogService;
        private readonly IViewNavigation _viewNavigation;
        private readonly IReceiptOrderRepository _receiptOrderRepository;
        private readonly IDocumentRepository _documentRepository;

        private readonly ObservableAsPropertyHelper<bool> _isLoadingProducts;
        private readonly ObservableAsPropertyHelper<IEnumerable<Product>> _products;
        private ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> _cancelCommand;
        private bool _popupSearchProductOpen;
        private ReceiptOrderEdit _receiptOrderEdit;
        private Product _addedProduct;

        public AddEditReceiptOrderViewModel(INotificationService notificationService, IUpdateTokenService updateTokenService,
            IProductRepository productRepository, IAccessTokenRepository accessTokenRepository, 
            IDialogService dialogService, IViewNavigation viewNavigation,
            IReceiptOrderRepository receiptOrderRepository,IDocumentRepository documentRepository) : base(notificationService, updateTokenService)
        {
            _productRepository = productRepository;
            _accessTokenRepository = accessTokenRepository;
            _dialogService = dialogService;
            _viewNavigation = viewNavigation;
            _receiptOrderRepository = receiptOrderRepository;
            _documentRepository = documentRepository;

            _lazyGetReceiptOrderCommand = new Lazy<ReactiveCommand<int, System.Reactive.Unit>>(() =>
            {
                var command = ReactiveCommand.CreateFromTask<int>(GetReceiptOrder);
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
                if(p!=null && p.Length > 0)
                {
                    RestartLoadProducts();
                    PopupSearchProductOpen = true;
                }
                else
                {
                    PopupSearchProductOpen = false;
                }
            });
            
            this.WhenAnyValue(x => x.AddedProduct).Subscribe(_ => AddProductForReceipt());
            RemoveProductForReceipt = ReactiveCommand.Create<ReceiptOrderProduct>(ExecuteRemoveProductForReceipt);

            this.WhenAnyValue(x => x.Bundle).Where(x => x != null).Throttle(TimeSpan.FromSeconds(0.3)).Subscribe((x) =>
            {
                int? index = (int?)x.GetParameter("idReceiptOrder");
                if (index != null)
                {
                    _lazyGetReceiptOrderCommand.Value.Execute(index.Value);
                }
                else
                {
                    ReceiptOrderEdit = new ReceiptOrderEdit();
                }
            });
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ReceiptOrderEdit.ReceiptOrderProduct.Count < 1)
                {
                    await _dialogService.ShowDialog("Сохранение", "Нельзя оформить приход без продукции.", IDialogService.DialogType.Standard);
                    return;
                }
                int receiptId = await _receiptOrderRepository.SaveReceiptOrder(_accessTokenRepository.GetAccessToken(), ReceiptOrderEdit);
                _lazyGetReceiptOrderCommand.Value.Execute(receiptId);
                (Bundle.OwnerViewModel as ReceiptOrderViewModel).RestartLoadReceiptOrders();
            });
            SaveCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, SaveCommand));
        }

        public bool IsLoadingProducts => _isLoadingProducts.Value;
        public readonly Lazy<ReactiveCommand<int, System.Reactive.Unit>> _lazyGetReceiptOrderCommand;
        public IEnumerable<Product> Products => _products.Value;
        public ReactiveCommand<System.Reactive.Unit, ProductsCollection> LoadingProducts { get; private set; }
        public ReactiveCommand<ReceiptOrderProduct, Unit> RemoveProductForReceipt { get; }
        public OwnersParameters OwnersParameters { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SaveCommand { get; private set; }
        public bool PopupSearchProductOpen
        {
            get => _popupSearchProductOpen;
            set => this.RaiseAndSetIfChanged(ref _popupSearchProductOpen, value);
        }
        public ReceiptOrderEdit ReceiptOrderEdit
        {
            get => _receiptOrderEdit;
            set => this.RaiseAndSetIfChanged(ref _receiptOrderEdit, value);
        }
        public Product AddedProduct
        {
            get => _addedProduct;
            set => this.RaiseAndSetIfChanged(ref _addedProduct, value);
        }

        private async Task<ProductsCollection> LoadingProductsTask(CancellationToken ct)
        {
            var accessToken = _accessTokenRepository.GetAccessToken();
            if (ct.IsCancellationRequested) return null;
            ProductsCollection? productsCollection;
            productsCollection = await _productRepository.GetProducts(accessToken, OwnersParameters);
            if (ct.IsCancellationRequested) return null;
            var productsWithoutSameProductId = productsCollection.Products
            .Where(product => !ReceiptOrderEdit.ReceiptOrderProduct.Any(orderProduct => orderProduct.Product.ProductId == product.ProductId))
            .ToList();
            productsCollection.Products = productsWithoutSameProductId;
            return productsCollection;
        }
        private async Task GetReceiptOrder(int id)
        {
            var receiptOrder = await _receiptOrderRepository.GetReceiptOrderEdit(_accessTokenRepository.GetAccessToken(), id);
            ReceiptOrderEdit = receiptOrder;
        }
        private void RestartLoadProducts()
        {
            _cancelCommand.Execute().Subscribe();
            LoadingProducts.Execute().Subscribe();
        }
        private void AddProductForReceipt()
        {
            if (AddedProduct != null)
            {
                ReceiptOrderEdit.ReceiptOrderProduct.Add(new ReceiptOrderProduct { Product = AddedProduct, Quantity = 1, 
                PurchasePrice = AddedProduct.PurchasePrice ?? 0});
                OwnersParameters.SearchString = "";
            }
        }
        private void ExecuteRemoveProductForReceipt(ReceiptOrderProduct receiptOrderProduct)
        {
            if (receiptOrderProduct != null)
            {
                ReceiptOrderEdit.ReceiptOrderProduct.Remove(receiptOrderProduct);
            }
        }
        public async void IssueReceipt()
        {
            if (ReceiptOrderEdit.ReceiptOrderProduct.Count < 1)
            {
                await _dialogService.ShowDialog("Сохранение", "Нельзя оформить приход без продукции.", IDialogService.DialogType.Standard);
                return;
            }
            ReceiptOrderEdit.IsReceipt = true;
            SaveCommand.Execute();
        }
        public void CancelReceipt()
        {
            ReceiptOrderEdit.IsReceipt = false;
            SaveCommand.Execute();
        }
        public void Close()
        {
            _viewNavigation.Close(this);
        }
        public async Task PrintPrihod()
        {
            await _documentRepository.GeneratePrihod(ReceiptOrderEdit);
        }
    }
}
