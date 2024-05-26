using desktop.Models;
using desktop.Services;
using desktop.Services.Repositories;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace desktop.ViewModels
{
    public class ExpenseOrdersViewModel : ViewModelBase
    {
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IExpenseOrderRepository _expenseOrderRepository;
        private readonly IViewNavigation _viewNavigation;



        private readonly ObservableAsPropertyHelper<bool> _isLoadingExpenseOrders;
        private readonly ObservableAsPropertyHelper<IEnumerable<ExpenseOrder>> _expenseOrders;
        private ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> _cancelCommand;
        private readonly ObservableAsPropertyHelper<int> _countPage;
        private readonly ObservableAsPropertyHelper<int> _countExpenseOrders;
        private bool _isExpenseSelected = false; //

        public ExpenseOrdersViewModel(INotificationService notificationService, IUpdateTokenService updateTokenService,
            IExpenseOrderRepository expenseOrderRepository, IAccessTokenRepository accessTokenRepository,
            IViewNavigation viewNavigation) : base(notificationService, updateTokenService)
        {
            _accessTokenRepository = accessTokenRepository;
            _expenseOrderRepository = expenseOrderRepository;
            _viewNavigation = viewNavigation;

            LoadingExpenseOrders = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(ct => this.LoadingExpenseOrdersTask(ct))
                                .TakeUntil(_cancelCommand));
            LoadingExpenseOrders.IsExecuting.ToProperty(this, x => x.IsLoadingExpenseOrders, out _isLoadingExpenseOrders);
            LoadingExpenseOrders.ThrownExceptions.Subscribe(async (x) => await CommandExc(x, LoadingExpenseOrders));
            _expenseOrders = LoadingExpenseOrders
                        .Where(p => p != null).Select(x=>x.ExpenseOrders)
                        .ToProperty(this, x => x.ExpenseOrders, scheduler: RxApp.MainThreadScheduler);
            _cancelCommand = ReactiveCommand.Create(() => { }, LoadingExpenseOrders.IsExecuting);

            _countExpenseOrders = LoadingExpenseOrders.Where(x => x != null).Select(x => x.Count).ToProperty(this, x => x.CountExpenseOrders, scheduler: RxApp.MainThreadScheduler);

            OwnersParameters = new OwnersParameters(); 
            this.WhenAnyValue(x => x.CountExpenseOrders, x => x.OwnersParameters.SizePage)
                .Where(x => x.Item2 != 0)
                .Select(x => (x.Item1 + x.Item2 - 1) / x.Item2)
                .ToProperty(this, x => x.CountPage, out _countPage);
            OwnersParameters.WhenAnyValue(p => p.PageNumber).Subscribe(_ => RestartLoadExpenseOrders());
            OwnersParameters.WhenAnyValue(p => p.SizePage).Subscribe(_ => GoToFirstPageAndRestartLoadExpenseOrders());
            OwnersParameters.WhenAnyValue(p => p.SearchString)
                            .Throttle(TimeSpan.FromSeconds(0.75)).Subscribe(_ => GoToFirstPageAndRestartLoadExpenseOrders());

            LoadingExpenseOrders.Execute().Subscribe();

            this.WhenAnyValue(x => x.IsExpenseSelected).Subscribe(_ => GoToFirstPageAndRestartLoadExpenseOrders()); //

            EditExpenseOrderCommand = ReactiveCommand.Create<int>(EditExpenseOrder);

            DateRange = new DateRange();
            DateRange.WhenAnyValue(x => x.DateOne, x => x.DateTwo).Subscribe(_ => GoToFirstPageAndRestartLoadExpenseOrders()); //

        }

        public bool IsLoadingExpenseOrders => _isLoadingExpenseOrders.Value;
        public ReactiveCommand<System.Reactive.Unit, ExpenseOrderCollection> LoadingExpenseOrders { get; private set; }
        public IEnumerable<ExpenseOrder> ExpenseOrders => _expenseOrders.Value;
        public ReactiveCommand<int, System.Reactive.Unit> EditExpenseOrderCommand { get; }
        public int CountPage => _countPage.Value; 
        public int CountExpenseOrders => _countExpenseOrders.Value; 
        public IEnumerable<int> PageCounts => new int[] { 10, 20, 50, 100 }; 
        public OwnersParameters OwnersParameters { get; set; }
        public DateRange DateRange { get; set; } //
        public bool IsExpenseSelected //
        {
            get => _isExpenseSelected;
            set => this.RaiseAndSetIfChanged(ref _isExpenseSelected, value);
        }
        private async Task<ExpenseOrderCollection> LoadingExpenseOrdersTask(CancellationToken ct)
        {
            var accessToken = _accessTokenRepository.GetAccessToken();
            if (ct.IsCancellationRequested) return null;
            var expenseOrdersCollection = await _expenseOrderRepository.GetExpenseOrders(accessToken,OwnersParameters);
            if (ct.IsCancellationRequested) return null;
            if (IsExpenseSelected) //
            {
                expenseOrdersCollection.ExpenseOrders = expenseOrdersCollection.ExpenseOrders.Where(x => x.IsExpense == IsExpenseSelected);
            }
            if(DateRange.DateOne != null && DateRange.DateTwo != null) //
            expenseOrdersCollection.ExpenseOrders = expenseOrdersCollection.ExpenseOrders
                .Where(x => x.DateOfCreate >= DateRange.DateOne && x.DateOfCreate <= DateRange.DateTwo);
            return expenseOrdersCollection;
        }
        public void RestartLoadExpenseOrders()
        {
            _cancelCommand.Execute().Subscribe();
            LoadingExpenseOrders.Execute().Subscribe();
        }
        public void AddExpenseOrder()
        {
            Bundle bundle = new Bundle(this);
            _viewNavigation.GoTo<AddEditExpenseOrderViewModel>(bundle);
        }
        private void EditExpenseOrder(int id)
        {
            Bundle bundle = new Bundle(this);
            bundle.AddNewParameter("idExpenseOrder", id);
            _viewNavigation.GoTo<AddEditExpenseOrderViewModel>(bundle);
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
        public void GoToFirstPageAndRestartLoadExpenseOrders() 
        {
            if (OwnersParameters.PageNumber == 1)
                RestartLoadExpenseOrders();
            else OwnersParameters.PageNumber = 1;
        }
        public void CancelDateSearch() //
        {
            DateRange.DateOne = null;
            DateRange.DateTwo = null;
        }
    }
}
