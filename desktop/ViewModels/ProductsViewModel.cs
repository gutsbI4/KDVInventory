using Avalonia.Controls.Notifications;
using Avalonia.Controls.Templates;
using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using desktop.Tools;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace desktop.ViewModels
{
    public class ProductsViewModel : ViewModelBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDialogService _dialogService;
        private readonly IViewNavigation _viewNavigation;
        private readonly IFilterRepository _filterRepository;

        private readonly ObservableAsPropertyHelper<bool> _isLoadingProducts;
        private readonly ObservableAsPropertyHelper<IEnumerable<Product>> _products;
        private ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> _cancelCommand;
        private Product _selectedProduct;
        private readonly ObservableAsPropertyHelper<bool> _isLoadingProductDetails;
        private readonly ObservableAsPropertyHelper<ProductDetails> _productDetails;
        private readonly ObservableAsPropertyHelper<int> _countPage;
        private readonly ObservableAsPropertyHelper<int> _countProducts;
        private readonly ObservableAsPropertyHelper<bool> _isSearch;
        private bool _firstLoadCompleted = false;
        private List<Category> _categories;
        private Category _selectedCategory;
        private List<FilterCategory> _filterCategories;
        private string _categoryName = "Все категории";
        private bool _isMoreProducts;
        private Category _category;
        private bool _isEmptyProducts;

        public ProductsViewModel(IProductRepository productRepository,
            IAccessTokenRepository accessTokenRepository,
            INotificationService notificationService,
            IUpdateTokenService updateTokenService,
            ICategoryRepository categoryRepository,
            IDialogService dialogService,
            IViewNavigation viewNavigation, IFilterRepository filterRepository) : base(notificationService, updateTokenService)
        {
            _productRepository = productRepository;
            _accessTokenRepository = accessTokenRepository;
            _categoryRepository = categoryRepository;
            _dialogService = dialogService;
            _viewNavigation = viewNavigation;
            _filterRepository = filterRepository;

            LoadingProducts = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(ct => this.LoadingProductsTask(ct))
                                .TakeUntil(_cancelCommand));
            LoadFiltersCommand = ReactiveCommand.CreateFromTask(LoadFilters);
            LoadingProducts.IsExecuting.ToProperty(this, x => x.IsLoadingProducts, out _isLoadingProducts);
            LoadingProducts.ThrownExceptions.Subscribe(async (x) => await CommandExc(x, LoadingProducts));
            _products = LoadingProducts
                        .Where(p => p != null)
                        .Select(x => x.Products)
                        .ToProperty(this, x => x.Products, scheduler: RxApp.MainThreadScheduler);
            LoadingProducts.Subscribe(result =>
            {
                if (result != null  && !_firstLoadCompleted)
                {
                    LoadFiltersCommand.Execute().Subscribe();
                    _firstLoadCompleted = true;
                }
            });
            _cancelCommand = ReactiveCommand.Create(() => { }, LoadingProducts.IsExecuting);

            _countProducts = LoadingProducts.Where(x => x != null).Select(x => x.Count).ToProperty(this, x => x.CountProducts, scheduler: RxApp.MainThreadScheduler);


            OwnersParameters = new OwnersParameters();
            this.WhenAnyValue(x => x.CountProducts, x => x.OwnersParameters.SizePage)
                .Where(x => x.Item2 != 0)
                .Select(x => (x.Item1 + x.Item2 - 1) / x.Item2)
                .ToProperty(this, x => x.CountPage, out _countPage);
            OwnersParameters.WhenAnyValue(p => p.PageNumber).Subscribe(_ => 
            {
                if(Filters == null)
                _firstLoadCompleted = false;
                RestartLoadProducts();
            });
            OwnersParameters.WhenAnyValue(p => p.SizePage).Subscribe(_ =>
            {
                _firstLoadCompleted = false;
                GoToFirstPageAndRestartLoadProducts();
            } );
            OwnersParameters.WhenAnyValue(p => p.Filter).Subscribe(_ => GoToFirstPageAndRestartLoadProducts());
            this.WhenAnyValue(x => x.CountPage).Subscribe(count =>
            {
                if (count == 1) IsMoreProducts = false;
                else IsMoreProducts = true;
            });
            this.WhenAnyValue(p => p.SelectedCategory).Subscribe(_ =>
            {
                if (SelectedCategory != null) 
                {
                    CategoryName = SelectedCategory.Name;
                    _category = SelectedCategory;
                    _firstLoadCompleted = false;
                    GoToFirstPageAndRestartLoadProducts();
                } 
            });
            OwnersParameters.WhenAnyValue(p => p.Sorts).Subscribe(_ => GoToFirstPageAndRestartLoadProducts());

            //Remove product
            RemoveProductCommand = ReactiveCommand.CreateFromTask<int>(RemoveProduct);
            RemoveProductCommand.ThrownExceptions.Subscribe(async exp => await CommandExc(exp, RemoveProduct));

            //EditProduct
            EditProductCommand = ReactiveCommand.Create<int>(EditProduct);
            EditProductCommand.ThrownExceptions.Subscribe(async exp => await CommandExc(exp, EditProductCommand));

            Search = ReactiveCommand.CreateFromTask(SearchTask);
            Search.IsExecuting.ToProperty(this, x => x.IsSearch, out _isSearch);

        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
        }
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
        }
        public List<Category> Categories
        {
            get => _categories;
            set => this.RaiseAndSetIfChanged(ref _categories, value);
        }
        public string CategoryName
        {
            get => _categoryName;
            set => this.RaiseAndSetIfChanged(ref _categoryName, value);
        }
        public bool IsMoreProducts
        {
            get => _isMoreProducts;
            set => this.RaiseAndSetIfChanged(ref _isMoreProducts, value);
        }
        public bool IsLoadingProducts => _isLoadingProducts.Value;
        public bool IsSearch => _isSearch.Value;
        public IEnumerable<Product> Products => _products.Value;
        public ReactiveCommand<System.Reactive.Unit, ProductsCollection> LoadingProducts { get; private set; }
        public bool IsLoadingProductDetails => _isLoadingProductDetails.Value;
        public ProductDetails SelectedProductDetails => _productDetails.Value;
        public ReactiveCommand<Product, ProductDetails> LoadingProductDetails { get; set; }
        public int CountPage => _countPage.Value;
        public int CountProducts => _countProducts.Value;
        public IEnumerable<int> PageCounts => new int[] { 10,15,20,30,40 };
        public OwnersParameters OwnersParameters { get; set; }
        public List<FilterCategory> Filters
        {
            get => _filterCategories;
            set => this.RaiseAndSetIfChanged(ref _filterCategories, value);
        }
        public ReactiveCommand <int, System.Reactive.Unit> RemoveProductCommand { get; set; }
        public ObservableCollection<SortElement> SortElements { get; private set; }
        public ReactiveCommand<int, System.Reactive.Unit> EditProductCommand { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> Search { get; private set; }
        public ReactiveCommand<Unit, Unit> LoadFiltersCommand { get; }
        public bool IsEmptyProducts
        {
            get => _isEmptyProducts;
            set => this.RaiseAndSetIfChanged(ref _isEmptyProducts, value);
        }

        public async Task SearchTask()
        {
            _firstLoadCompleted = false;
            GoToFirstPageAndRestartLoadProducts();
        }
        public void AddProduct()
        {
            Bundle bundle = new Bundle(this);
            _viewNavigation.GoTo<AddEditProductViewModel>(bundle);
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
        private async Task<ProductsCollection> LoadingProductsTask(CancellationToken ct)
        {
            var accessToken = _accessTokenRepository.GetAccessToken();
            if (ct.IsCancellationRequested) return null;
            ProductsCollection? productsCollection;
            if(_category!= null) productsCollection = await _productRepository.GetProducts(accessToken, OwnersParameters, _category.Id);
            else productsCollection = await _productRepository.GetProducts(accessToken, OwnersParameters);
            if (ct.IsCancellationRequested) return null;
            SelectedProduct = productsCollection.Products.Count() > 0 ? productsCollection.Products.ToList()[0] : null;
            if (productsCollection.Count == 0) IsEmptyProducts = true;
            else IsEmptyProducts = false;
            return productsCollection;
        }
        private void RestartLoadProducts()
        {
            _cancelCommand.Execute().Subscribe();
            LoadingProducts.Execute().Subscribe();
        }
        public void GoToFirstPageAndRestartLoadProducts()
        {
            if (OwnersParameters.PageNumber == 1)
                RestartLoadProducts();
            else OwnersParameters.PageNumber = 1;
        }
        public void LoadMoreProducts()
        {
            OwnersParameters.SizePage += 10;
        }
        private List<FilterParameter> GetFilterParameters()
        {
            List<FilterParameter> filterParameters = new List<FilterParameter>();
            foreach(FilterCategory filterCategory in Filters)
            {
                if (filterCategory.Filters == null)
                    break;
                foreach (Filter filter in filterCategory.Filters.Where(p => p.IsPick))
                {
                    filterParameters.Add(new FilterParameter()
                    {
                        NameParameter = filterCategory.ParameterName, Value = filter.Value
                    });
                }
            }
            return filterParameters;
        }
        private async Task LoadFilters()
        {
            var categories = await _categoryRepository.GetCategories();
            Categories = categories.ToList();
            Filters = await _filterRepository.GetFilters(Products.ToList());
            Filters.ForEach(x => x.Filters.ToObservableChangeSet()
                .AutoRefresh(a => a.IsPick)
                .Subscribe(_ =>
                {
                    var filterParameters = GetFilterParameters();
                    OwnersParameters.Filter = JsonSerializer.Serialize(filterParameters);
                }));
        }
        private async Task RemoveProduct(int id)
        {
            var result = await _dialogService.ShowDialog("Удаление", "Вы действительно хотите удалить продукт?"
                , IDialogService.DialogType.YesNoDialog);
            if(result == IDialogService.DialogResult.Yes)
            {
                await _productRepository.DeleteProduct(_accessTokenRepository.GetAccessToken(), id);
                GoToFirstPageAndRestartLoadProducts();
            }
        }
        private void EditProduct(int id)
        {
            Bundle bundle = new Bundle(this);
            bundle.AddNewParameter("idProduct", id);
            _viewNavigation.GoTo<AddEditProductViewModel>(bundle);
        }
    }
}
