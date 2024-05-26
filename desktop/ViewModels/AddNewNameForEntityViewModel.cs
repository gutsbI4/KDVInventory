using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.ViewModels
{
    public class AddNewNameForEntityViewModel : ViewModelBase
    {
        private readonly IViewNavigation _viewNavigation;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IPriceUnitRepository _priceUnitRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IDietRepository _dietRepository;
        private readonly IFillingRepository _fillingRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly ITasteRepository _tasteRepository;
        private readonly ITypeRepository _typeRepository;
        private readonly IDialogService _dialogService;

        public AddNewNameForEntityViewModel(INotificationService notificationService, IUpdateTokenService updateTokenService,
            IAccessTokenRepository accessTokenRepository, IBrandRepository brandRepository,
            IDietRepository dietRepository, IFillingRepository fillingRepository, IManufacturerRepository manufacturerRepository,
            IPackageRepository packageRepository, IPriceUnitRepository priceUnitRepository,
            ITasteRepository tasteRepository, ITypeRepository typeRepository, IViewNavigation viewNavigation,
            IDialogService dialogService
            ) : base(notificationService, updateTokenService)
        {
            _viewNavigation = viewNavigation;
            _accessTokenRepository = accessTokenRepository;
            _manufacturerRepository = manufacturerRepository;
            _priceUnitRepository = priceUnitRepository;
            _brandRepository = brandRepository;
            _dietRepository = dietRepository;
            _fillingRepository = fillingRepository;
            _packageRepository = packageRepository;
            _tasteRepository = tasteRepository;
            _typeRepository = typeRepository;
            _dialogService = dialogService;

            this.WhenAnyValue(x => x.Bundle).Where(x => x != null).Throttle(TimeSpan.FromSeconds(0.3)).Subscribe((x) =>
            {
                string? entity = (string?)x.GetParameter("entity");
                if (entity != null)
                {
                    Title = entity;
                }
                else
                {
                    Title = "Ошибка";
                }
            });

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = Bundle?.OwnerViewModel as AddEditProductViewModel;
                switch (Title)
                {
                    case "Вид":
                        await _typeRepository.AddType(_accessTokenRepository.GetAccessToken(),Name);
                        vm.GetTypesCommand.Execute().Subscribe();
                        break;
                    case "Единица измерения":
                        await _priceUnitRepository.AddPriceUnit(_accessTokenRepository.GetAccessToken(), Name);
                        vm.GetPriceUnitsCommand.Execute().Subscribe();
                        break;
                    case "Производитель":
                        await _manufacturerRepository.AddManufacturer(_accessTokenRepository.GetAccessToken(), Name);
                        vm.GetManufacturersCommand.Execute().Subscribe();
                        break;
                    case "Торговая марка":
                        await _brandRepository.AddBrand(_accessTokenRepository.GetAccessToken(), Name);
                        vm.GetBrandsCommand.Execute().Subscribe();
                        break;
                    case "Упаковка":
                        await _packageRepository.AddPackage(_accessTokenRepository.GetAccessToken(), Name);
                        vm.GetPackagesCommand.Execute().Subscribe();
                        break;
                    case "Вкус":
                        await _tasteRepository.AddTaste(_accessTokenRepository.GetAccessToken(), Name);
                        vm.GetTastesCommand.Execute().Subscribe();
                        break;
                    case "Начинка":
                        await _fillingRepository.AddFilling(_accessTokenRepository.GetAccessToken(), Name);
                        vm.GetFillingsCommand.Execute().Subscribe();
                        break;
                    case "Диета":
                        await _dietRepository.AddDiet(_accessTokenRepository.GetAccessToken(), Name);
                        vm.GetDietsCommand.Execute().Subscribe();
                        break;
                }
                _viewNavigation.Close(this);
           });
            SaveCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, SaveCommand));
        }
        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SaveCommand { get; private set; }

        public async void Exit()
        {
            if (!String.IsNullOrEmpty(Name))
            {
                var result = await _dialogService.ShowDialog("Внимание",
                    "На странице имеются несохранённые данные. Вы хотите уйти со страницы без сохранения изменений?",
                    IDialogService.DialogType.YesNoDialog);
                if (result == IDialogService.DialogResult.Yes) _viewNavigation.Close(this);
                else return;
            }
            _viewNavigation.Close(this);
        }
    }

}
