using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Models;
using Models.Attributes;
using Models.Management;
using Models.Messages;
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

        private ProcessesType CurrentProcessesType;

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
        private bool _hasUrgent = false;

        [ObservableProperty]
        private ObservableCollection<EntityFilterItem> _activeFilters = new ObservableCollection<EntityFilterItem>();

        [ObservableProperty]
        private ObservableCollection<EntityFilterItem> _filterOptions = new ObservableCollection<EntityFilterItem>();

        [ObservableProperty]
        private ObservableCollection<ListItem> _urgentItems = new ObservableCollection<ListItem>();

        [ObservableProperty]
        private bool _showUrgent = false;

        [ObservableProperty]
        private ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();

        [NotifyPropertyChangedFor(nameof(IsDraft))]
        [NotifyPropertyChangedFor(nameof(IsNotDraft))]
        [NotifyPropertyChangedFor(nameof(HasSearchBar))]
        [NotifyPropertyChangedFor(nameof(HasCreateButton))]
        [NotifyPropertyChangedFor(nameof(HasDuplicateButton))]
        [NotifyPropertyChangedFor(nameof(HasEditButton))]
        [NotifyPropertyChangedFor(nameof(HasDeleteButton))]
        [NotifyPropertyChangedFor(nameof(HasCancelButton))]
        [NotifyPropertyChangedFor(nameof(HasSaveButton))]
        [ObservableProperty]
        private ListItem? _selectedItem;

        // 1. Only true if an item is actively selected AND its database record says it's a draft
        public bool IsDraft => SelectedItem != null && (TopEntity?.IsDraft ?? false);

        // 2. Only true if an item is actively selected AND it is a finalized record
        public bool IsNotDraft => SelectedItem != null && !IsDraft;


        public bool HasSearchBar => EnableSearchBar;
        public bool HasCreateButton => EnableCreateButton;
        public bool HasDuplicateButton => EnableDuplicateButton && IsNotDraft; // Optional: hides duplicate if nothing is selected

        // 3. Non-Draft Buttons (Show only for finalized selections)
        public bool HasEditButton => EnableEditButton && IsNotDraft;
        public bool HasDeleteButton => EnableDeleteButton && IsNotDraft;

        // 4. Draft Buttons (FIXED: Changed from IsNotDraft to IsDraft!)
        public bool HasCancelButton => EnableCancelButton && IsDraft;
        public bool HasSaveButton => EnableSaveButton && IsDraft;

        [ObservableProperty]
        private bool _enableSearchBar = true;

        [ObservableProperty]
        private bool _enableCreateButton = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasEditButton))]
        private bool _enableEditButton = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasDeleteButton))]
        private bool _enableDeleteButton = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasCancelButton))]
        private bool _enableCancelButton = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSaveButton))]
        private bool _enableSaveButton = true;

        [ObservableProperty]
        private bool _enableDuplicateButton = true;

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
            WeakReferenceMessenger.Default.Register<SaveDraftMessage>(this, async (recipient, message) =>
            {
                await SaveDraft();
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
                case ProcessesType.Customers:
                    Title = "Customers";
                    Description = "Customers";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    FilterOptions.Add(new EntityFilterItem(FilterType.Retail));
                    FilterOptions.Add(new EntityFilterItem(FilterType.Wholesale));
                    break;
                case ProcessesType.Orders:
                    Title = "Orders";
                    Description = "Orders";
                    HasUrgent = true;
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    FilterOptions.Add(new EntityFilterItem(FilterType.Complete));
                    FilterOptions.Add(new EntityFilterItem(FilterType.Incomplete));
                    FilterOptions.Add(new EntityFilterItem(FilterType.InStore));
                    FilterOptions.Add(new EntityFilterItem(FilterType.Online));
                    break;
                case ProcessesType.Products:
                    Title = "Products";
                    Description = "Products";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.Users:
                    Title = "Users";
                    Description = "Users";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.ProductCategories:
                    Title = "Categories";
                    Description = "Categories";
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
                case ProcessesType.StitchTypes:
                    Title = "Stitch Types";
                    Description = "Stitch Types";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.YarnColors:
                    Title = "Yarn Colors";
                    Description = "Yarn Colors";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.Fabrics:
                    Title = "Fabrics";
                    Description = "Fabrics";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.Brands:
                    Title = "Brands";
                    Description = "Brands";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.DropOffApt:
                    Title = "Drop Off";
                    Description = "Appointments";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.TestTryApt:
                    Title = "Test Try";
                    Description = "Appointments";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.PickUpApt:
                    Title = "Pick Up";
                    Description = "Appointments";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    break;
                case ProcessesType.FoamFix:
                    Title = "Fix Foam";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.FoamAdapt:
                    Title = "Adapt Foam";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.FoamGel:
                    Title = "Gel";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.FoamAnatomical:
                    Title = "Anatomical Foam";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.CoverRemove:
                    Title = "Remove Cover";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.CustomPattern:
                    Title = "Custom Patterns";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.Cut:
                    Title = "Cutting";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.Sew:
                    Title = "Sewing";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.Embroider:
                    Title = "Embroider";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.Bolt:
                    Title = "Bolt";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.Inspect:
                    Title = "Inspect";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.Tasks:
                    Title = "All Tasks";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.Foam:
                    Title = "Foam Tasks";
                    Description = "Tasks";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
                    break;
                case ProcessesType.Calendar:
                    Title = "Calendar";
                    Description = "Appointments";
                    HasUrgent = true;
                    EnableCreateButton = false;
                    EnableDeleteButton = false;
                    EnableDuplicateButton = false;
                    EnableCancelButton = false;
                    EnableSaveButton = false;
                    EnableSearchBar = false;
                    EnableEditButton = false;
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
                    Items.Add(new ListItem((dynamic)item));

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

        private async void PushToStack(IEntity entity)
        {
            if (entity == null) { return; }
            if (TopEntity != null)
            {
                EntityStack.Push(TopEntity);
            }
            UpdatePreviousPageTitle();
            TopEntity = entity;
            OnPropertyChanged(nameof(HasBackButton));
            await UpdateDetailsView();
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
            await UpdateDetailsView();
        }

        private async Task UpdateTopEntity()
        {
            if (TopEntity == null) { return; }
            TopEntity = await restService.Sync(TopEntity);
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
                    Orders o => $"Order #O-{o.Id}",
                    Products p => $"Product #P-{p.Id}",
                    Users p => p.Name ?? "Unknown User",
                    Models.Attributes.Models m => m.ModelName ?? "Unknown Model",
                    Fabrics p => p.FabricName ?? "Unknown Fabric",
                    Patterns p => p.Name ?? "Unknown Pattern",
                    StitchTypes p => p.StitchTypeName ?? "Unknown Stitch Type",
                    YarnColors p => p.YarnColorName ?? "Unknown Yarn Color",
                    Brands p => p.BrandName ?? "Unknown Brand",
                    ProductCategories pc => pc.CategoryName ?? "Unknown Category",
                    Tasks t => $"Task #T-{t.Id}",
                    _ => String.Empty,
                };
            }
        }


        private async Task UpdateDetailsView()
        {
            if (TopEntity == null)
            {
                ActiveDetailView = null;
                return;
            }
            await UpdateTopEntity();
            try
            {
                ActiveDetailView = (dynamic)TopEntity switch
                {
                    Customers => ServiceHelper.GetService<CustomersView>(),
                    Orders => ServiceHelper.GetService<OrdersView>(),
                    StitchTypes => ServiceHelper.GetService<StitchTypesView>(),
                    YarnColors => ServiceHelper.GetService<YarnColorsView>(),
                    Fabrics => ServiceHelper.GetService<FabricsView>(),
                    Patterns => ServiceHelper.GetService<PatternsView>(),
                    ProductCategories => ServiceHelper.GetService<ProductCategoriesView>(),
                    Brands => ServiceHelper.GetService<BrandsView>(),
                    Users => ServiceHelper.GetService<UsersView>(),
                    Models.Attributes.Models => ServiceHelper.GetService<ModelsView>(),
                    Products => ServiceHelper.GetService<ProductsView>(),
                    Tasks => ServiceHelper.GetService<TasksView>(),
                    _ => throw new NotImplementedException(),
                };
            }
            catch
            {
                await RefreshItems();
                return;
            }
            
            switch ((dynamic)ActiveDetailView)
            {
                case CustomersView customersView:
                    customersView.LoadEntity((Customers)TopEntity);
                    break;
                case OrdersView ordersView:
                    ordersView.LoadEntity((Orders)TopEntity);
                    break;
                case StitchTypesView stitchTypesView:
                    stitchTypesView.LoadEntity((StitchTypes)TopEntity);
                    break;
                case YarnColorsView yarnColorsView:
                    yarnColorsView.LoadEntity((YarnColors)TopEntity);
                    break;
                case FabricsView fabricsView:
                    fabricsView.LoadEntity((Fabrics)TopEntity);
                    break;
                case PatternsView patternsView:
                    patternsView.LoadEntity((Patterns)TopEntity);
                    break;
                case ProductCategoriesView productCategoriesView:
                    productCategoriesView.LoadEntity((ProductCategories)TopEntity);
                    break;
                case BrandsView brandsView:
                    brandsView.LoadEntity((Brands)TopEntity);
                    break;
                case UsersView usersView:
                    usersView.LoadEntity((Users)TopEntity);
                    break;
                case ModelsView modelsView:
                    modelsView.LoadEntity((Models.Attributes.Models)TopEntity);
                    break;
                case ProductsView productsView:
                    productsView.LoadEntity((Products)TopEntity);
                    break;
                case TasksView tasksView:
                    tasksView.LoadEntity((Tasks)TopEntity);
                    break;
            }
            OnPropertyChanged(nameof(IsDraft));
            OnPropertyChanged(nameof(IsNotDraft));
            OnPropertyChanged(nameof(HasEditButton));
            OnPropertyChanged(nameof(HasDeleteButton));
            OnPropertyChanged(nameof(HasCancelButton));
            OnPropertyChanged(nameof(HasSaveButton));
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
            await RefreshItems();
            await UpdateDetailsView();
        }

        [RelayCommand]
        public async Task Duplicate()
        {
            if (TopEntity == null || TopEntity.FromId < 0) { return; }
            TopEntity.IsDraft = true;
            TopEntity.FromId = 0;
            var newEntity = await restService.Post((dynamic)TopEntity);
            TopEntity = newEntity;
            await UpdateDetailsView();
        }

        [RelayCommand]
        public async Task Cancel()
        {
            if (TopEntity == null || TopEntity.FromId < 0) { return; }
            await restService.DeleteEntity(TopEntity);
            await RefreshItems();
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
                Users => await restService.Get<Users>(TopEntity.FromId),
                Models.Attributes.Models => await restService.Get<Models.Attributes.Models>(TopEntity.FromId),
                ProductCategories => await restService.Get<ProductCategories>(TopEntity.FromId),
                Fabrics => await restService.Get<Fabrics>(TopEntity.FromId),
                Brands => await restService.Get<Brands>(TopEntity.FromId),
                Patterns => await restService.Get<Patterns>(TopEntity.FromId),
                StitchTypes => await restService.Get<StitchTypes>(TopEntity.FromId),
                YarnColors => await restService.Get<YarnColors>(TopEntity.FromId),
                Tasks => await restService.Get<Tasks>(TopEntity.FromId),
                _ => throw new NotImplementedException(),
            };
            
            await UpdateDetailsView();
        }

        [RelayCommand]
        public async Task Save()
        {
            if (IsBusy) {  return; }
            try
            {
                IsBusy = true;
                if (TopEntity == null) { return; }
                ActiveDetailView?.SaveEntity();
                TopEntity.IsDraft = false;
                if (TopEntity.FromId > 0)
                {
                    if (TopEntity.Id > 0)
                    {
                        await restService.DeleteEntity(TopEntity);
                    }
                    TopEntity.Id = TopEntity.FromId;
                    await restService.Put((dynamic)TopEntity);
                }
                else if (TopEntity.Id == 0)
                {
                    await restService.Post((dynamic)TopEntity);
                }
                else
                {
                    await restService.Put((dynamic)TopEntity);
                }


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SAVE COMMAND CRASHED]: {ex.Message}");
            }
            finally
            {

                IsBusy = false;
                await RefreshItems();
                await UpdateDetailsView();
            }


        }

        public async Task SaveDraft()
        {
            if (TopEntity == null || !TopEntity.IsDraft) { return; }
            ActiveDetailView?.SaveEntity();

            if (TopEntity.Id == 0)
            {
                restService.Post((dynamic)TopEntity);
            }
            else
            {
                restService.Put((dynamic)TopEntity);
            }
        }

        [RelayCommand]
        public async Task Create()
        {
            await DeselectItem();
            await ClearStack();
            IEntity? entity;
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
                case ProcessesType.Users:
                    entity = new Users() { IsDraft = true, Name = string.Empty, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.Models:
                    entity = new Models.Attributes.Models() { IsDraft = true, ModelName = string.Empty, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.ProductCategories:
                    entity = new ProductCategories() { IsDraft = true, CategoryName = string.Empty, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.Patterns:
                    entity = new Patterns() { IsDraft = true, Name = string.Empty, ModelId = 0, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.Tasks:
                    entity = null;
                    break;
                case ProcessesType.StitchTypes:
                    entity = new StitchTypes() { IsDraft = true, StitchTypeName = string.Empty, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.YarnColors:
                    entity = new YarnColors() { IsDraft = true, YarnColorName = string.Empty, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.Fabrics:
                    entity = new Fabrics() { IsDraft = true, FabricName = string.Empty, CreatedDate = DateTime.Now };
                    break;
                case ProcessesType.Brands:
                    entity = new Brands() { IsDraft = true, BrandName = string.Empty, CreatedDate = DateTime.Now };
                    break;
                default:
                    throw new NotImplementedException($"Creation not supported for type {CurrentProcessesType}");
            }
            if (entity == null) { return; }
            var response = await restService.Post((dynamic)entity);
            entity.Id = response.Id;
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
                ProcessesType.ProductCategories => await restService.Get<ProductCategories>(parameters),
                ProcessesType.Models => await restService.Get<Models.Attributes.Models>(parameters),
                ProcessesType.Patterns => await restService.Get<Patterns>(parameters),
                ProcessesType.StitchTypes => await restService.Get<StitchTypes>(parameters),
                ProcessesType.YarnColors => await restService.Get<YarnColors>(parameters),
                ProcessesType.Fabrics => await restService.Get<Fabrics>(parameters),
                ProcessesType.DropOffApt => await restService.GetAvailableTasks(12),
                ProcessesType.TestTryApt => await restService.GetAvailableTasks(13),
                ProcessesType.PickUpApt => await restService.GetAvailableTasks(14),
                ProcessesType.FoamFix => await restService.GetAvailableTasks(15),
                ProcessesType.FoamAdapt => await restService.GetAvailableTasks(16),
                ProcessesType.FoamGel => await restService.GetAvailableTasks(17),
                ProcessesType.FoamAnatomical => await restService.GetAvailableTasks(18),
                ProcessesType.CoverRemove => await restService.GetAvailableTasks(19),
                ProcessesType.CustomPattern => await restService.GetAvailableTasks(20),
                ProcessesType.Cut => await restService.GetAvailableTasks(21),
                ProcessesType.Sew => await restService.GetAvailableTasks(22),
                ProcessesType.Embroider => await restService.GetAvailableTasks(23),
                ProcessesType.Bolt => await restService.GetAvailableTasks(24),
                ProcessesType.Inspect => await restService.GetAvailableTasks(25),
                ProcessesType.Tasks => await restService.GetAvailableTasks(12),
                ProcessesType.Foam => await restService.GetAvailableTasks(12),
                ProcessesType.Calendar => await restService.GetAvailableTasks(12),
                ProcessesType.Brands => await restService.Get<Brands>(parameters),
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
