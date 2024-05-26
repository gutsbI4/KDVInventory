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
    public class ReceiptOrderViewModel : ViewModelBase
    {
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IReceiptOrderRepository _receiptOrderRepository;
        private readonly IViewNavigation _viewNavigation;



        private readonly ObservableAsPropertyHelper<bool> _isLoadingReceiptOrders;
        private readonly ObservableAsPropertyHelper<IEnumerable<ReceiptOrder>> _receiptOrders;
        private ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> _cancelCommand;
        private readonly ObservableAsPropertyHelper<int> _countPage;
        private readonly ObservableAsPropertyHelper<int> _countReceiptOrders;
        private bool _isReceiptSelected = false; //

        public ReceiptOrderViewModel(INotificationService notificationService, IUpdateTokenService updateTokenService,
            IReceiptOrderRepository receiptOrderRepository, IAccessTokenRepository accessTokenRepository,
            IViewNavigation viewNavigation) : base(notificationService, updateTokenService)
        {
            _accessTokenRepository = accessTokenRepository;
            _receiptOrderRepository = receiptOrderRepository;
            _viewNavigation = viewNavigation;

            LoadingReceiptOrders = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(ct => this.LoadingReceiptOrdersTask(ct))
                                .TakeUntil(_cancelCommand));
            LoadingReceiptOrders.IsExecuting.ToProperty(this, x => x.IsLoadingReceiptOrders, out _isLoadingReceiptOrders);
            LoadingReceiptOrders.ThrownExceptions.Subscribe(async (x) => await CommandExc(x, LoadingReceiptOrders));
            _receiptOrders = LoadingReceiptOrders
                        .Where(p => p != null).Select(x=>x.ReceiptOrders)
                        .ToProperty(this, x => x.ReceiptOrders, scheduler: RxApp.MainThreadScheduler);

            _countReceiptOrders = LoadingReceiptOrders.Where(x => x != null).Select(x => x.Count).ToProperty(this, x => x.CountReceiptOrders, scheduler: RxApp.MainThreadScheduler);

            
            _cancelCommand = ReactiveCommand.Create(() => { }, LoadingReceiptOrders.IsExecuting);

            OwnersParameters = new OwnersParameters();
            this.WhenAnyValue(x => x.CountReceiptOrders, x => x.OwnersParameters.SizePage)
                .Where(x => x.Item2 != 0)
                .Select(x => (x.Item1 + x.Item2 - 1) / x.Item2)
                .ToProperty(this, x => x.CountPage, out _countPage);
            OwnersParameters.WhenAnyValue(p => p.PageNumber).Subscribe(_ => RestartLoadReceiptOrders());
            OwnersParameters.WhenAnyValue(p => p.SizePage).Subscribe(_ => GoToFirstPageAndRestartLoadReceiptOrders());
            OwnersParameters.WhenAnyValue(p => p.SearchString)
                            .Throttle(TimeSpan.FromSeconds(0.75)).Subscribe(_ => GoToFirstPageAndRestartLoadReceiptOrders());

            LoadingReceiptOrders.Execute().Subscribe();

            EditReceiptOrderCommand = ReactiveCommand.Create<int>(EditReceiptOrder);

            this.WhenAnyValue(x => x.IsReceiptSelected).Subscribe(_ => GoToFirstPageAndRestartLoadReceiptOrders()); //
            DateRange = new DateRange();
            DateRange.WhenAnyValue(x => x.DateOne, x => x.DateTwo).Subscribe(_ => GoToFirstPageAndRestartLoadReceiptOrders()); //
        }

        public bool IsLoadingReceiptOrders => _isLoadingReceiptOrders.Value;
        public ReactiveCommand<System.Reactive.Unit, ReceiptOrderCollection> LoadingReceiptOrders { get; private set; }
        public IEnumerable<ReceiptOrder> ReceiptOrders => _receiptOrders.Value;
        public ReactiveCommand<int, System.Reactive.Unit> EditReceiptOrderCommand { get; }
        public int CountPage => _countPage.Value; 
        public int CountReceiptOrders => _countReceiptOrders.Value;
        public IEnumerable<int> PageCounts => new int[] { 10, 20, 50, 100 };
        public OwnersParameters OwnersParameters { get; set; }
        public DateRange DateRange { get; set; } //
        public bool IsReceiptSelected //
        {
            get => _isReceiptSelected;
            set => this.RaiseAndSetIfChanged(ref _isReceiptSelected, value);
        }

        private async Task<ReceiptOrderCollection> LoadingReceiptOrdersTask(CancellationToken ct)
        {
            var accessToken = _accessTokenRepository.GetAccessToken();
            if (ct.IsCancellationRequested) return null;
            var receiptOrdersCollection = await _receiptOrderRepository.GetReceiptOrders(accessToken,OwnersParameters);
            if (ct.IsCancellationRequested) return null;
            if (IsReceiptSelected) //
            {
                receiptOrdersCollection.ReceiptOrders = receiptOrdersCollection.ReceiptOrders.Where(x => x.IsReceipt == IsReceiptSelected);
            }
            if (DateRange.DateOne != null && DateRange.DateTwo != null) //
                receiptOrdersCollection.ReceiptOrders = receiptOrdersCollection.ReceiptOrders
                    .Where(x => x.DateOfCreate >= DateRange.DateOne && x.DateOfCreate <= DateRange.DateTwo);
            return receiptOrdersCollection;
        }
        public void RestartLoadReceiptOrders()
        {
            _cancelCommand.Execute().Subscribe();
            LoadingReceiptOrders.Execute().Subscribe();
        }
        public void AddReceiptOrder()
        {
            Bundle bundle = new Bundle(this);
            _viewNavigation.GoTo<AddEditReceiptOrderViewModel>(bundle);
        }
        private void EditReceiptOrder(int id)
        {
            Bundle bundle = new Bundle(this);
            bundle.AddNewParameter("idReceiptOrder", id);
            _viewNavigation.GoTo<AddEditReceiptOrderViewModel>(bundle);
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
        public void GoToFirstPageAndRestartLoadReceiptOrders()
        {
            if (OwnersParameters.PageNumber == 1)
                RestartLoadReceiptOrders();
            else OwnersParameters.PageNumber = 1;
        }
        public void CancelDateSearch() //
        {
            DateRange.DateOne = null;
            DateRange.DateTwo = null;
        }
    }
}
