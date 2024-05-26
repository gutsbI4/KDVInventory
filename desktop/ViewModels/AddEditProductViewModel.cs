using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using Force.DeepCloner;
using ReactiveUI;
using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace desktop.ViewModels
{
    public class AddEditProductViewModel : ViewModelBase
    {
        private readonly IViewNavigation _viewNavigation;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPriceUnitRepository _priceUnitRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IDietRepository _dietRepository;
        private readonly IFillingRepository _fillingRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly ITasteRepository _tasteRepository;
        private readonly ITypeRepository _typeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUpdateTokenService _updateTokenService;
        private readonly INotificationService _notificationService;
        private readonly IFilePickerService _filePickerService;
        private readonly IImageService _imageService;
        private readonly IDialogService _dialogService;
        private bool _isFirstLoad = true;

        public AddEditProductViewModel(IViewNavigation viewNavigation, IAccessTokenRepository accessTokenRepository,
            IManufacturerRepository manufacturerRepository, ICategoryRepository categoryRepository, IPriceUnitRepository priceUnitRepository,
            IBrandRepository brandRepository, IDietRepository dietRepository, IFillingRepository fillingRepository,
            IPackageRepository packageRepository, ITasteRepository tasteRepository, ITypeRepository typeRepository,
            IProductRepository productRepository, IUpdateTokenService updateTokenService, INotificationService notificationService,
            IFilePickerService filePickerService, IImageService imageService, IDialogService dialogService) : base(notificationService, updateTokenService)
        {
            _viewNavigation = viewNavigation;
            _accessTokenRepository = accessTokenRepository;
            _manufacturerRepository = manufacturerRepository;
            _categoryRepository = categoryRepository;
            _priceUnitRepository = priceUnitRepository;
            _brandRepository = brandRepository;
            _dietRepository = dietRepository;
            _fillingRepository = fillingRepository;
            _packageRepository = packageRepository;
            _tasteRepository = tasteRepository;
            _typeRepository = typeRepository;
            _productRepository = productRepository;
            _updateTokenService = updateTokenService;
            _notificationService = notificationService;
            _filePickerService = filePickerService;
            _imageService = imageService;
            _dialogService = dialogService;

            _lazyGetProductCommand = new Lazy<ReactiveCommand<int, System.Reactive.Unit>>(() =>
            {
                var command = ReactiveCommand.CreateFromTask<int>(GetProduct);
                command.ThrownExceptions.Subscribe(async x => CommandExc(x, command));
                return command;
            });
            GetCategoriesCommand = ReactiveCommand.CreateFromTask(async () => await _categoryRepository.GetCategories());
            GetCategoriesCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetCategoriesCommand));
            _categories = GetCategoriesCommand.ToProperty(this, x => x.Categories);
            GetCategoriesCommand.Execute().Subscribe();;

            GetManufacturersCommand = ReactiveCommand.CreateFromTask(async () => await _manufacturerRepository.GetManufacturers());
            GetManufacturersCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetManufacturersCommand));
            _manufacturers = GetManufacturersCommand.ToProperty(this, x => x.Manufacturers);
            this.WhenAnyValue(vm => vm.Manufacturers)
            .Where(manufacturers => manufacturers != null && manufacturers.Any())
            .Subscribe(manufacturers =>
            {
                if (!_isFirstLoad && Product != null)
                {
                    Product.Details.ManufacturerId = manufacturers.Last().Id;
                }
            });
            GetManufacturersCommand.Execute().Subscribe();

            GetPriceUnitsCommand = ReactiveCommand.CreateFromTask(async () => await _priceUnitRepository.GetPriceUnits());
            GetPriceUnitsCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetPriceUnitsCommand));
            _priceUnits = GetPriceUnitsCommand.ToProperty(this, x => x.PriceUnits);
            this.WhenAnyValue(vm => vm.PriceUnits)
            .Where(priceUnits => priceUnits != null && priceUnits.Any())
            .Subscribe(priceUnits =>
            {
                if (!_isFirstLoad && Product != null)
                {
                    Product.PriceUnitId = priceUnits.Last().Id;
                }
            });
            GetPriceUnitsCommand.Execute().Subscribe();

            GetBrandsCommand = ReactiveCommand.CreateFromTask(async () => await _brandRepository.GetBrands());
            GetBrandsCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetBrandsCommand));
            _brands = GetBrandsCommand.ToProperty(this, x => x.Brands);
            this.WhenAnyValue(vm => vm.Brands)
            .Where(brands => brands != null && brands.Any())
            .Subscribe(brands =>
            {
                if (!_isFirstLoad && Product != null)
                {
                    Product.Details.BrandId = brands.Last().BrandId;
                }
            });
            GetBrandsCommand.Execute().Subscribe();

            GetDietsCommand = ReactiveCommand.CreateFromTask(async () => await _dietRepository.GetDiets());
            GetDietsCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetDietsCommand));
            _diets = GetDietsCommand.ToProperty(this, x => x.Diets);
            this.WhenAnyValue(vm => vm.Diets)
            .Where(diets => diets != null && diets.Any())
            .Subscribe(diets =>
            {
                if (!_isFirstLoad && Product != null)
                {
                    Product.Details.DietId = diets.Last().DietId;
                }
            });
            GetDietsCommand.Execute().Subscribe();

            GetFillingsCommand = ReactiveCommand.CreateFromTask(async () => await _fillingRepository.GetFillings());
            GetFillingsCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetFillingsCommand));
            _fillings = GetFillingsCommand.ToProperty(this, x => x.Fillings);
            this.WhenAnyValue(vm => vm.Fillings)
            .Where(fillings => fillings != null && fillings.Any())
            .Subscribe(fillings =>
            {
                if (!_isFirstLoad && Product != null)
                {
                    Product.Details.FillingId = fillings.Last().FillingId;
                }
            });
            GetFillingsCommand.Execute().Subscribe();
            
            GetPackagesCommand = ReactiveCommand.CreateFromTask(async () => await _packageRepository.GetPackages());
            GetPackagesCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetPackagesCommand));
            _packages = GetPackagesCommand.ToProperty(this, x => x.Packages);
            this.WhenAnyValue(vm => vm.Packages)
            .Where(packages => packages != null && packages.Any())
            .Subscribe(packages =>
            {
                if (!_isFirstLoad && Product != null)
                {
                    Product.Details.PackageId = packages.Last().PackageId;
                }
            });
            GetPackagesCommand.Execute().Subscribe();

            GetTastesCommand = ReactiveCommand.CreateFromTask(async () => await _tasteRepository.GetTastes());
            GetTastesCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetTastesCommand));
            _tastes = GetTastesCommand.ToProperty(this, x => x.Tastes);
            this.WhenAnyValue(vm => vm.Tastes)
            .Where(tastes => tastes != null && tastes.Any())
            .Subscribe(tastes =>
            {
                if (!_isFirstLoad && Product != null)
                {
                    Product.Details.TasteId = tastes.Last().TasteId;
                }
            });
            GetTastesCommand.Execute().Subscribe();

            GetTypesCommand = ReactiveCommand.CreateFromTask(async () => await _typeRepository.GetTypes());
            GetTypesCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, GetTypesCommand));
            _types = GetTypesCommand.ToProperty(this, x => x.Types);
            this.WhenAnyValue(vm => vm.Types)
            .Where(types => types != null && types.Any())
            .Subscribe(types =>
            {
                if (!_isFirstLoad && Product != null)
                {
                    Product.Details.TypeId = types.Last().TypeId;
                }
                _isFirstLoad = false;
            });
            GetTypesCommand.Execute().Subscribe();

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Bundle.GetParameter("idProduct") != null)
                    await _productRepository.UpdateProduct(_accessTokenRepository.GetAccessToken(), Product);
                else await _productRepository.AddProduct(_accessTokenRepository.GetAccessToken(), Product);
                (Bundle?.OwnerViewModel as ProductsViewModel)?.GoToFirstPageAndRestartLoadProducts();
                _viewNavigation.Close(this);
            });
            SaveCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, SaveCommand));

            _isProductModified = false;
            Observable.Merge(
                    this.WhenAnyValue(x => x.Product.Name).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Description).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.PriceOfSale).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Barcode).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x =>x.Product.ProductNumber).Skip(1).Select(_=>Unit.Default),
                    this.WhenAnyValue(x => x.Product.CategoryId).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.MinPriceOfSale).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.PurchasePrice).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.LowStockThreshold).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.PriceUnitId).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.ProductImage).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.BrandId).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.ManufacturerId).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.CountryOfProduction).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.ShelfLife).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.Weight).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.PackageId).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.TypeId).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.TasteId).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.FillingId).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Details.DietId).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Energy.Proteins).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Energy.Fats).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Energy.Carbs).Skip(1).Select(_ => Unit.Default),
                    this.WhenAnyValue(x => x.Product.Energy.Calories).Skip(1).Select(_ => Unit.Default)
                ).Subscribe(_ =>
                {
                    _isProductModified = true;
                });

            this.WhenAnyValue(x => x.Bundle).Where(x => x != null).Throttle(TimeSpan.FromSeconds(0.3)).Subscribe((x) =>
            {
                int? index = (int?)x.GetParameter("idProduct");
                if (index != null)
                {
                    Title = "Редактирование товара";
                    ButtonSaveText = "Сохранить";
                    _lazyGetProductCommand.Value.Execute(index.Value);
                }
                else
                {
                    Title = "Добавление товара";
                    ButtonSaveText = "Добавить";
                    Product = new ProductEdit();
                    Product.Details = new ProductDetails();
                    Product.Energy = new EnergyValue();
                }
            });
            AddImageProductCommand = ReactiveCommand.CreateFromTask(AddImageProduct);
            AddImageProductCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, AddImageProductCommand));
            RemoveImageCommand = ReactiveCommand.Create<int>((idToDelete) =>
            {
                var updatedImages = Product.ProductImage?.ToList() ?? new List<ProductImage>();

                var imageToRemove = updatedImages.FirstOrDefault(image => image.ImageId == idToDelete);

                if (imageToRemove != null)
                {
                    updatedImages.Remove(imageToRemove);
                }

                Product.ProductImage = updatedImages;
            });
            GenerateBarcodeCommand = ReactiveCommand.CreateFromTask(GenerateBarcode);
            GenerateBarcodeCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, GenerateBarcodeCommand));

            GenerateArticleCommand = ReactiveCommand.CreateFromTask(GenerateArticle);
            GenerateArticleCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, GenerateArticleCommand));

            AddNewNameForEntityCommand = ReactiveCommand.Create<string>(AddNewNameForEntity);
            AddNewNameForEntityCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, AddNewNameForEntityCommand));

        }

        private ProductEdit _product;
        private ProductEdit _productBeforeRefreshLists;
        public readonly Lazy<ReactiveCommand<int, System.Reactive.Unit>> _lazyGetProductCommand;
        private readonly ObservableAsPropertyHelper<IEnumerable<Manufacturer>> _manufacturers;
        private readonly ObservableAsPropertyHelper<IEnumerable<Category>> _categories;
        private readonly ObservableAsPropertyHelper<IEnumerable<Models.PriceUnit>> _priceUnits;
        private readonly ObservableAsPropertyHelper<IEnumerable<Brand>> _brands;
        private readonly ObservableAsPropertyHelper<IEnumerable<Diet>> _diets;
        private readonly ObservableAsPropertyHelper<IEnumerable<Filling>> _fillings;
        private readonly ObservableAsPropertyHelper<IEnumerable<Package>> _packages;
        private readonly ObservableAsPropertyHelper<IEnumerable<Taste>> _tastes;
        private readonly ObservableAsPropertyHelper<IEnumerable<Models.Type>> _types;
        private readonly ObservableAsPropertyHelper<IEnumerable<ProductImage>> _productImages;
        private string _textSearchIngredients;
        private string _buttonSaveText;
        private bool _isProductModified = false;

        public IEnumerable<Manufacturer> Manufacturers => _manufacturers.Value;
        public IEnumerable<Category> Categories => _categories.Value;
        public IEnumerable<Models.PriceUnit> PriceUnits => _priceUnits.Value;
        public IEnumerable<Brand> Brands => _brands.Value;
        public IEnumerable<Diet> Diets => _diets.Value;
        public IEnumerable<Filling> Fillings => _fillings.Value;
        public IEnumerable<Package> Packages => _packages.Value;
        public IEnumerable<Taste> Tastes => _tastes.Value;
        public IEnumerable<Models.Type> Types => _types.Value;
        public IEnumerable<ProductImage> ProductImages => _productImages.Value;
        public ReactiveCommand<System.Reactive.Unit, IEnumerable<Manufacturer>> GetManufacturersCommand { get; private set; }
        public ReactiveCommand<System.Reactive.Unit, IEnumerable<Category>> GetCategoriesCommand { get; private set; }
        public ReactiveCommand<System.Reactive.Unit, IEnumerable<Models.PriceUnit>> GetPriceUnitsCommand { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<Brand>> GetBrandsCommand { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<Diet>> GetDietsCommand { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<Filling>> GetFillingsCommand { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<Package>> GetPackagesCommand { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<Taste>> GetTastesCommand { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<Models.Type>> GetTypesCommand { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<ProductImage>> GetProductImagesCommand { get; private set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SaveCommand { get; private set; }
        public ReactiveCommand<Unit, System.Reactive.Unit> AddImageProductCommand { get; }
        public ReactiveCommand<int, System.Reactive.Unit> RemoveImageCommand { get; }
        public ReactiveCommand<Unit, Unit> GenerateBarcodeCommand { get; private set; }
        public ReactiveCommand<Unit,Unit> GenerateArticleCommand { get; private set; }
        public ReactiveCommand<string, Unit> AddNewNameForEntityCommand { get; }
        public ProductEdit Product
        {
            get => _product;
            set => this.RaiseAndSetIfChanged(ref _product, value);
        }
        public string ButtonSaveText
        {
            get => _buttonSaveText;
            set => this.RaiseAndSetIfChanged(ref _buttonSaveText, value);
        }
        private async Task GetProduct(int id)
        {
            var product = await _productRepository.GetProductEdit(_accessTokenRepository.GetAccessToken(), id);
            Product = product;
            if (Product.Details == null) Product.Details = new ProductDetails();
            if (Product.Energy == null) Product.Energy = new EnergyValue();
        }
        private async Task AddImageProduct()
        {
            await using var stream = await _filePickerService.OpenFile(IFilePickerService.Filter.JpgImage);
            if (stream == null)
                return;
            StreamPart file = new StreamPart(stream, "image.jpg", contentType: "image/jpeg");
            string fileName = await _imageService.UploadImage(_accessTokenRepository.GetAccessToken(), file);
            var newProductImage = new ProductImage
            {
                Path = fileName,
                ProductId = Product.ProductId
            };
            var updatedImages = Product.ProductImage?.ToList() ?? new List<ProductImage>();
            updatedImages.Add(newProductImage);
            Product.ProductImage = updatedImages;
        }
        private async Task GenerateBarcode()
        {
            long barcode = await _productRepository.GetRandomBarcode(_accessTokenRepository.GetAccessToken());
            Product.Barcode = barcode;
        }
        private async Task GenerateArticle()
        {
            string article = await _productRepository.GetRandomArticle(_accessTokenRepository.GetAccessToken());
            Product.ProductNumber = article;
        }
        public async void Exit()
        {
            if (_isProductModified)
            {
                var result = await _dialogService.ShowDialog("Внимание",
                    "На странице имеются несохранённые данные. Вы хотите уйти со страницы без сохранения изменений?",
                    IDialogService.DialogType.YesNoDialog);
                if (result == IDialogService.DialogResult.Yes) _viewNavigation.Close(this);
                else return;
            }
            _viewNavigation.Close(this);
        }
        private void AddNewNameForEntity(string entity)
        {
            Bundle bundle = new Bundle(this);
            bundle.AddNewParameter("entity", entity);
            _viewNavigation.GoTo<AddNewNameForEntityViewModel>(bundle);

        }
    }
}
