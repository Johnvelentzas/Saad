using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Models;
using Models.Attributes;
using Models.Finances;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Messages;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.Views.DetailsViews;
using System.Collections.ObjectModel;

namespace Producion_Line_Manager.ViewModels
{
    public partial class TabListViewModel : BaseViewModel
    {

        private readonly RestService restService;

        private Models.Production.ProcessesType CurrentProcessesType;

        private int Page = 1;
        private int TotalPages = 1;
        private int PageSize = 20;

        [ObservableProperty]
        private TabItem? _currentTab;

        [ObservableProperty]
        private ObservableCollection<EntitySortItem> _sortOptions = new ObservableCollection<EntitySortItem>();

        [ObservableProperty]
        private EntitySortItem _activeSort = new EntitySortItem();

        [ObservableProperty]
        private int _itemNumber = 0;

        [ObservableProperty]
        private string _description = String.Empty;

        [ObservableProperty]
        private int _urgentItemNumber = 0;

        [ObservableProperty]
        private string _urgentDescription = "Urgent";

        [ObservableProperty]
        private ObservableCollection<EntityFilterItem> _activeFilters = new ObservableCollection<EntityFilterItem>();

        [ObservableProperty]
        private ObservableCollection<EntityFilterItem> _filterOptions = new ObservableCollection<EntityFilterItem>();

        [ObservableProperty]
        private ObservableCollection<ListItem> _urgentItems = new ObservableCollection<ListItem>();

        [ObservableProperty]
        private ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();

        [NotifyPropertyChangedFor(nameof(IsDraft))]
        [NotifyPropertyChangedFor(nameof(IsNotDraft))]
        [ObservableProperty]
        private ListItem? _selectedItem;

        public bool IsDraft => TopEntity?.IsDraft ?? false;
        public bool IsNotDraft => !IsDraft;

        [ObservableProperty]
        private bool _hasSearchBar = true;

        [ObservableProperty]
        private bool _hasCreateButton = true;

        [ObservableProperty]
        private SearchType _searchType = SearchType.General;

        [ObservableProperty]
        private string _searchQuerry = String.Empty;

        private CancellationTokenSource? _searchCancellationTokenSource;

        private PeriodicTimer? _autoRefreshTimer;
        private CancellationTokenSource? _autoRefreshCts;

        [ObservableProperty]
        private string _previousPageTitle = String.Empty;

        public bool HasBackButton => _entityStack.Count > 0;

        [ObservableProperty]
        private Stack<IEntity> _entityStack = new Stack<IEntity>();

        [ObservableProperty]
        private IEntity? _topEntity;

        [ObservableProperty]
        private BaseEntityView? _activeDetailView;

        public TabListViewModel()
        {
            Title = "List View";
            restService = ServiceHelper.GetService<RestService>();
            StartAutoRefresh();
            WeakReferenceMessenger.Default.Register<TabListViewModel, OpenEntityMessage>(this, (recipient, message) =>
            {
                recipient.PushToStack(message.Value);
            });
        }


        public void StartAutoRefresh()
        {
            StopAutoRefresh();

            _autoRefreshCts = new CancellationTokenSource();

            _autoRefreshTimer = new PeriodicTimer(TimeSpan.FromMinutes(15));

            _ = RunRefreshLoopAsync(_autoRefreshCts.Token);
        }

        public void StopAutoRefresh()
        {
            _autoRefreshCts?.Cancel();
            _autoRefreshTimer?.Dispose();
        }

        private async Task RunRefreshLoopAsync(CancellationToken token)
        {
            try
            {
                while (await _autoRefreshTimer!.WaitForNextTickAsync(token))
                {
                    await RefreshItems();
                }
            }
            catch (OperationCanceledException)
            {
                // The loop was safely cancelled
            }
        }

        async partial void OnSearchQuerryChanged(string value)
        {
            if (value.Length < 4 && value.Length > 0) { return; }
            _searchCancellationTokenSource?.Cancel();
            _searchCancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Delay(1000, _searchCancellationTokenSource.Token);
                await RefreshItems();
            }
            catch (TaskCanceledException)
            {

            }
        }

        async partial void OnActiveSortChanged(EntitySortItem value)
        {
            await RefreshItems();
        }

        [RelayCommand]
        public async Task OpenTab(TabItem tab)
        {
            if (tab == null)
            {
                return;
            }

            CurrentTab = tab;
            await SetItemSource();
            await RefreshItems();
        }

        [RelayCommand]
        public async Task SetItemSource()
        {
            if (CurrentTab == null) { return; }
            CurrentProcessesType = CurrentTab.Type;
            SortOptions.Add(new EntitySortItem(SortType.IdDecending));
            SortOptions.Add(new EntitySortItem(SortType.IdAccending));
            ActiveSort = SortOptions[0];
            switch (CurrentTab.Type)
            {
                case Models.Production.ProcessesType.Customers:
                    Title = "Customers";
                    Description = "Customers";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    FilterOptions.Add(new EntityFilterItem(FilterType.Retail));
                    FilterOptions.Add(new EntityFilterItem(FilterType.Wholesale));
                    break;
                case Models.Production.ProcessesType.Orders:
                    Title = "Orders";
                    Description = "Orders";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case Models.Production.ProcessesType.Products:
                    Title = "Products";
                    Description = "Products";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.Users:
                    Title = "Users";
                    Description = "Users";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.Models:
                    Title = "Models";
                    Description = "Models";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.Patterns:
                    Title = "Patterns";
                    Description = "Patterns";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.ProductCategories:
                    Title = "Categories";
                    Description = "Categories";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.PickUpApt:
                    break;
                case ProcessesType.FoamFix:
                    break;
                case ProcessesType.FoamAdapt:
                    break;
                case ProcessesType.FoamGel:
                    break;
                case ProcessesType.FoamAnatomical:
                    break;
                case ProcessesType.CoverRemove:
                    break;
                case ProcessesType.CustomPattern:
                    break;
                case ProcessesType.Cut:
                    break;
                case ProcessesType.Sew:
                    break;
                case ProcessesType.Embroider:
                    break;
                case ProcessesType.Bolt:
                    break;
                case ProcessesType.Inspect:
                    break;
                case ProcessesType.DeliverApt:
                    break;
                case ProcessesType.Tasks:
                    Title = "Tasks";
                    Description = "Tasks";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.Foam:
                    break;
                case ProcessesType.Calendar:
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        public async Task RefreshItems()
        {

            Page = 1;
            TotalPages = 1;
            Items.Clear();
            UrgentItems.Clear();
            await LoadMoreItems();
        }

        [RelayCommand]
        public async Task LoadMoreItems()
        {
            if (IsBusy) { return; }
            if (Page > TotalPages) { return; }
            IsBusy = true;
            try
            {
                List<FilterType> filters = new List<FilterType>();
                foreach (var filter in ActiveFilters)
                {
                    filters.Add(filter.Type);
                }
                var parameters = new RequestParameters(
                    filters,
                    SearchType,
                    SearchQuerry,
                    Page,
                    PageSize,
                    ActiveSort.Type);
                IRequestResult? result = await ItemGetMethod(parameters);
                if (result == null)
                {
                    IsBusy = false;
                    return;
                }
                TotalPages = result.TotalPages;
                ItemNumber = result.TotalCount;
                foreach (var item in result.Items)
                {
                    if (item is Customers customers) { Items.Add(new ListItem(customers)); }
                    if (item is Orders orders) { Items.Add(new ListItem(orders)); }
                    if (item is Products products) { Items.Add(new ListItem(products)); }
                    if (item is Models.Attributes.Models models) { Items.Add(new ListItem(models)); }
                    if (item is ProductCategories categories) { Items.Add(new ListItem(categories)); }
                    if (item is Tasks task) { Items.Add(new ListItem(task)); }
                }
                Page++;
            }
            finally
            {
                IsBusy = false;
            }
            
        }

        [RelayCommand]
        public async Task SelectItem(ListItem item)
        {
            if(SelectedItem != null)
            {
                SelectedItem.IsSelected = false;
            }
            SelectedItem = item;
            SelectedItem.IsSelected = true;
            await ClearStack();
            PushToStack(SelectedItem.Entity);
        }

        [RelayCommand]
        public async Task DeselectItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.IsSelected = false;
            }
            SelectedItem = null;
            ActiveDetailView = null;
            await ClearStack();
        }

        private async Task ClearStack()
        {
            EntityStack.Clear();
            TopEntity = null;
            PreviousPageTitle = String.Empty;
            OnPropertyChanged(nameof(HasBackButton));
        }

        private void PushToStack(IEntity entity)
        {
            if (entity == null) { return; }
            if (TopEntity != null)
            {
                EntityStack.Push(TopEntity);
            }
            UpdatePreviousPageTitle();
            TopEntity = entity;
            OnPropertyChanged(nameof(HasBackButton));
            UpdateDetailsView();
        }

        [RelayCommand]
        public async Task GoBack()
        {
            if (EntityStack.Count > 0)
            {
                TopEntity = EntityStack.Pop();
            }
            else
            {
                TopEntity = null;
            }
            UpdatePreviousPageTitle();
            UpdateDetailsView();
        }

        private void UpdatePreviousPageTitle()
        {
            OnPropertyChanged(nameof(HasBackButton));
            if (EntityStack.Count == 0)
            {
                PreviousPageTitle = String.Empty;
            }
            else
            {
                IEntity previous = EntityStack.Peek();
                if (previous == null) { return; }

                PreviousPageTitle = previous switch
                {
                    Customers c => c.LastName ?? "No Last Name Customer",
                    Orders o => $"Order #{o.Id}",
                    Products p => $"Product #{p.Id}",
                    Models.Attributes.Models m => m.ModelName ?? "Unknown Model",
                    ProductCategories pc => pc.CategoryName ?? "Unknown Category",
                    Tasks t => $"Task #{t.Id}",
                    _ => String.Empty,
                };
            }
        }


        private void UpdateDetailsView()
        {
            if (TopEntity == null)
            {
                ActiveDetailView = null;
            }
            if(TopEntity == null) { return; }
            ActiveDetailView = (dynamic)TopEntity switch
            {
                Customers => ServiceHelper.GetService<CustomersView>(),
                Orders => ServiceHelper.GetService<OrdersView>(),
                _ => throw new NotImplementedException(),
            };
            switch ((dynamic)ActiveDetailView)
            {
                case CustomersView customersView:
                    customersView.LoadEntity((Customers)TopEntity);
                    break;
            }
            OnPropertyChanged(nameof(IsDraft));
            OnPropertyChanged(nameof(IsNotDraft));
        }

        [RelayCommand]
        public async Task SelectItemIndex(int  index)
        {
            if(index < 0 || index >= Items.Count) { return; }
            await SelectItem(Items[index]);

        }



        [RelayCommand]
        public async Task DeleteItem(ListItem item)
        {
            if (item == null) { return; }
            await restService.DeleteEntity(item.Entity);
            await RefreshItems();
        }

        [RelayCommand]
        public async Task Delete()
        {
            if (TopEntity == null) { return; }
            await restService.DeleteEntity(TopEntity);
            await GoBack();
            await RefreshItems();
        }


        [RelayCommand]
        public async Task Edit()
        {
            if (TopEntity == null || TopEntity.FromId < 0) { return; }
            TopEntity.IsDraft = true;
            TopEntity.FromId = TopEntity.Id;
            var newEntity = await restService.Post((dynamic)TopEntity);
            TopEntity = newEntity;
            UpdateDetailsView();
        }

        [RelayCommand]
        public async Task Cancel()
        {
            if (TopEntity == null || TopEntity.FromId < 0) { return; }
            await restService.DeleteEntity(TopEntity);
            if (TopEntity.FromId == 0)
            {
                await GoBack();
                return;
            }
            TopEntity = TopEntity switch
            {
                Customers => await restService.Get<Customers>(TopEntity.FromId),
                Orders => await restService.Get<Orders>(TopEntity.FromId),
                Products => await restService.Get<Products>(TopEntity.FromId),
                Models.Attributes.Models => await restService.Get<Models.Attributes.Models>(TopEntity.FromId),
                ProductCategories => await restService.Get<ProductCategories>(TopEntity.FromId),
                Tasks => await restService.Get<Tasks>(TopEntity.FromId),
                _ => throw new NotImplementedException(),
            };
            await RefreshItems();
            UpdateDetailsView();
        }

        [RelayCommand]
        public async Task Save()
        {
            if (TopEntity == null) { return; }
            ActiveDetailView?.SaveEntity();
            TopEntity.IsDraft = false;
            if (TopEntity.FromId > 0)
            {
                if(TopEntity.Id > 0)
                {
                    await restService.DeleteEntity(TopEntity);
                }
                TopEntity.Id = TopEntity.FromId;
                restService.Put((dynamic)TopEntity);
            }
            else if (TopEntity.Id == 0)
            {
                restService.Post((dynamic)TopEntity);
            }
            else
            {
                restService.Put((dynamic)TopEntity);
            }

            await RefreshItems();
            UpdateDetailsView();
        }

        [RelayCommand]
        public async Task Create()
        {
            await DeselectItem();
            await ClearStack();
            IEntity entity;
            switch (CurrentProcessesType)
            {
                case ProcessesType.Customers:
                    entity = new Customers() { IsDraft = true, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.Orders:
                    entity = new Orders() { IsDraft = true, CustomerId = 0, IsCompleted = false, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.Products:
                    entity = new Products() { IsDraft = true, CategoryId = 0, OrderId = 0, ExpectedFinishDate = DateTime.Now, ExpectedStartDate = DateTime.Now, IsCompleted = false, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.Models:
                    entity = new Models.Attributes.Models() { IsDraft = true, ModelName = string.Empty, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.ProductCategories:
                    entity = new ProductCategories() { IsDraft = true, CategoryName = string.Empty, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.Tasks:
                    entity = new Tasks() { IsDraft = true, ProcessId = 0, ProductId = 0, IsCompleted = false, CreatedDate = DateTime.Now };
                    break;
                default:
                    throw new NotImplementedException($"Creation not supported for type {CurrentProcessesType}");
            }
            restService.Post((dynamic)entity);
            PushToStack(entity);
            await RefreshItems();
        }


        private async Task<IRequestResult?> ItemGetMethod(RequestParameters parameters)
        {
            if (restService == null) { return null; }
            return CurrentProcessesType switch
            {
                ProcessesType.Customers => await restService.Get<Customers>(parameters),
                ProcessesType.Orders => await restService.Get<Orders>(parameters),
                ProcessesType.Products => await restService.Get<Products>(parameters),
                ProcessesType.Users => await restService.Get<Users>(parameters),
                ProcessesType.Models => await restService.Get<Models.Attributes.Models>(parameters),
                ProcessesType.Patterns => throw new NotImplementedException(),
                ProcessesType.ProductCategories => await restService.Get<ProductCategories>(parameters),
                ProcessesType.PickUpApt => throw new NotImplementedException(),
                ProcessesType.FoamFix => throw new NotImplementedException(),
                ProcessesType.FoamAdapt => throw new NotImplementedException(),
                ProcessesType.FoamGel => throw new NotImplementedException(),
                ProcessesType.FoamAnatomical => throw new NotImplementedException(),
                ProcessesType.CoverRemove => throw new NotImplementedException(),
                ProcessesType.CustomPattern => throw new NotImplementedException(),
                ProcessesType.Cut => throw new NotImplementedException(),
                ProcessesType.Sew => throw new NotImplementedException(),
                ProcessesType.Embroider => throw new NotImplementedException(),
                ProcessesType.Bolt => throw new NotImplementedException(),
                ProcessesType.Inspect => throw new NotImplementedException(),
                ProcessesType.DeliverApt => throw new NotImplementedException(),
                ProcessesType.Tasks => await restService.Get<Tasks>(parameters),
                ProcessesType.Foam => throw new NotImplementedException(),
                ProcessesType.Calendar => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }

        [RelayCommand]
        public async Task ActivateFilter(EntityFilterItem filter)
        {
            FilterOptions.Remove(filter);
            ActiveFilters.Add(filter);
            List<EntityFilterItem> incompatible = new();
            foreach (FilterType inc in filter.Incompatible)
            {
                foreach(var active in ActiveFilters)
                {
                    if(active.Type == inc)
                    {
                        incompatible.Add(active);
                    }
                }
            }
            foreach (var item in incompatible)
            {
                FilterOptions.Add(item);
                ActiveFilters.Remove(item);
            }
            await RefreshItems();
        }

        [RelayCommand]
        public async Task DeactivateFilter(EntityFilterItem filter)
        {
            FilterOptions.Add(filter);
            ActiveFilters.Remove(filter);
            await RefreshItems();
        }

    }
}
