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
    public class AddEditExpenseOrderViewModel : ViewModelBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IDialogService _dialogService;
        private readonly IViewNavigation _viewNavigation;
        private readonly IExpenseOrderRepository _expenseOrderRepository;
        private readonly IDocumentRepository _documentRepository;

        private readonly ObservableAsPropertyHelper<bool> _isLoadingProducts;
        private readonly ObservableAsPropertyHelper<IEnumerable<Product>> _products;
        private ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> _cancelCommand;
        private bool _popupSearchProductOpen;
        private ExpenseOrderEdit _expenseOrderEdit;
        private Product _addedProduct;

        public AddEditExpenseOrderViewModel(INotificationService notificationService, IUpdateTokenService updateTokenService,
            IProductRepository productRepository, IAccessTokenRepository accessTokenRepository,
            IDialogService dialogService, IViewNavigation viewNavigation,
            IExpenseOrderRepository expenseOrderRepository,IDocumentRepository documentRepository) : base(notificationService, updateTokenService)
        {
            _productRepository = productRepository;
            _accessTokenRepository = accessTokenRepository;
            _dialogService = dialogService;
            _viewNavigation = viewNavigation;
            _expenseOrderRepository = expenseOrderRepository;
            _documentRepository = documentRepository;

            _lazyGetExpenseOrderCommand = new Lazy<ReactiveCommand<int, System.Reactive.Unit>>(() =>
            {
                var command = ReactiveCommand.CreateFromTask<int>(GetExpenseOrder);
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

            this.WhenAnyValue(x => x.AddedProduct).Subscribe(_ => AddProductForExpense());
            RemoveProductForExpense = ReactiveCommand.Create<ExpenseOrderProduct>(ExecuteRemoveProductForExpense);

            this.WhenAnyValue(x => x.Bundle).Where(x => x != null).Throttle(TimeSpan.FromSeconds(0.3)).Subscribe((x) =>
            {
                int? index = (int?)x.GetParameter("idExpenseOrder");
                if (index != null)
                {
                    _lazyGetExpenseOrderCommand.Value.Execute(index.Value);
                }
                else
                {
                    ExpenseOrderEdit = new ExpenseOrderEdit();
                }
            });
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ExpenseOrderEdit.ExpenseOrderProduct.Count < 1)
                {
                    await _dialogService.ShowDialog("Сохранение", "Нельзя добавить списание без продукции.", IDialogService.DialogType.Standard);
                    return;
                }
                else if (ExpenseOrderEdit.Commentary == null || ExpenseOrderEdit.Commentary.Length == 0)
                {
                    await _dialogService.ShowDialog("Сохранение", "Нельзя списать товар без причины.", IDialogService.DialogType.Standard);
                    return;
                }
                int expenseId = await _expenseOrderRepository.SaveExpenseOrder(_accessTokenRepository.GetAccessToken(), ExpenseOrderEdit);
                _lazyGetExpenseOrderCommand.Value.Execute(expenseId);
                (Bundle.OwnerViewModel as ExpenseOrdersViewModel).RestartLoadExpenseOrders();
            });
            SaveCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, SaveCommand));
        }

        public bool IsLoadingProducts => _isLoadingProducts.Value;
        public readonly Lazy<ReactiveCommand<int, System.Reactive.Unit>> _lazyGetExpenseOrderCommand;
        public IEnumerable<Product> Products => _products.Value;
        public ReactiveCommand<System.Reactive.Unit, ProductsCollection> LoadingProducts { get; private set; }
        public ReactiveCommand<ExpenseOrderProduct, Unit> RemoveProductForExpense { get; }
        public OwnersParameters OwnersParameters { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SaveCommand { get; private set; }
        public bool PopupSearchProductOpen
        {
            get => _popupSearchProductOpen;
            set => this.RaiseAndSetIfChanged(ref _popupSearchProductOpen, value);
        }
        public ExpenseOrderEdit ExpenseOrderEdit
        {
            get => _expenseOrderEdit;
            set => this.RaiseAndSetIfChanged(ref _expenseOrderEdit, value);
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
            .Where(product => !ExpenseOrderEdit.ExpenseOrderProduct.Any(orderProduct => orderProduct.Product.ProductId == product.ProductId))
            .ToList();
            productsCollection.Products = productsWithoutSameProductId;
            return productsCollection;
        }
        private async Task GetExpenseOrder(int id)
        {
            var expenseOrder = await _expenseOrderRepository.GetExpenseOrderEdit(_accessTokenRepository.GetAccessToken(), id);
            ExpenseOrderEdit = expenseOrder;
        }
        private void RestartLoadProducts()
        {
            _cancelCommand.Execute().Subscribe();
            LoadingProducts.Execute().Subscribe();
        }
        private void AddProductForExpense()
        {
            if (AddedProduct != null)
            {
                ExpenseOrderEdit.ExpenseOrderProduct.Add(new ExpenseOrderProduct
                {
                    Product = AddedProduct,
                    Quantity = 1,
                    Price = AddedProduct.PurchasePrice ?? 0
                });
                OwnersParameters.SearchString = "";
            }
        }
        private void ExecuteRemoveProductForExpense(ExpenseOrderProduct expenseOrderProduct)
        {
            if (expenseOrderProduct != null)
            {
                ExpenseOrderEdit.ExpenseOrderProduct.Remove(expenseOrderProduct);
            }
        }
        public async void IssueExpense()
        {
            if (ExpenseOrderEdit.ExpenseOrderProduct.Count < 1)
            {
                await _dialogService.ShowDialog("Сохранение", "Нельзя добавить списание без продукции.", IDialogService.DialogType.Standard);
                return;
            }
            else if (ExpenseOrderEdit.Commentary == null || ExpenseOrderEdit.Commentary.Length == 0)
            {
                await _dialogService.ShowDialog("Сохранение", "Нельзя списать товар без причины.", IDialogService.DialogType.Standard);
                return;
            }
            ExpenseOrderEdit.IsExpense = true;
            SaveCommand.Execute();
        }
        public void CancelExpense()
        {
            ExpenseOrderEdit.IsExpense = false;
            SaveCommand.Execute();
        }
        public void Close()
        {
            _viewNavigation.Close(this);
        }
        public async Task PrintSpisanie()
        {
            await _documentRepository.GenerateSpisanie(ExpenseOrderEdit);
        }
    }
}
