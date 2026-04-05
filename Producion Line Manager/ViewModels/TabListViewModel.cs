using Producion_Line_Manager.Services;
using Producion_Line_Manager.Helpers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Finances;
using Models;
using Models.Production;
using Models.Attributes;

namespace Producion_Line_Manager.ViewModels
{
    public partial class TabListViewModel : BaseViewModel
    {

        private readonly RestService restService;

        [ObservableProperty]
        private ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();

        private int Page = 1;
        private int TotalPages = 1;
        private int PageSize = 100;

        [ObservableProperty]
        private ObservableCollection<ListItem> _urgentItems = new ObservableCollection<ListItem>();

        [ObservableProperty]
        private int _itemNumber = 0;

        [ObservableProperty]
        private string _description = String.Empty;

        [ObservableProperty]
        private int _urgentItemNumber = 0;

        [ObservableProperty]
        private string _urgentDescription = "Urgent";

        [ObservableProperty]
        private ListItem? _selectedItem;

        [ObservableProperty]
        private TabItem? _currentTab;

        private Models.Production.ProcessesType CurrentProcessesType;

        [ObservableProperty]
        private ContentView? _activeDetailView;

        [ObservableProperty]
        private ObservableCollection<EntitySortItem> _sortOptions = new ObservableCollection<EntitySortItem>();

        [ObservableProperty]
        private EntitySortItem _activeSort = new EntitySortItem();

        [ObservableProperty]
        private ObservableCollection<EntityFilterItem> _filterOptions = new ObservableCollection<EntityFilterItem>();

        [ObservableProperty]
        private ObservableCollection<EntityFilterItem> _activeFilters = new ObservableCollection<EntityFilterItem>();

        [ObservableProperty]
        private SearchType _searchType = SearchType.General;

        [ObservableProperty]
        private string _searchQuerry = String.Empty;

        public TabListViewModel()
        {
            Title = "List View";
            restService = ServiceHelper.GetService<RestService>();
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
            switch (CurrentTab.Type)
            {
                case Models.Production.ProcessesType.Customers:
                    Title = "Customers";
                    Description = "Customers";
                    FilterOptions.Add(new EntityFilterItem(FilterType.Draft));
                    FilterOptions.Add(new EntityFilterItem(FilterType.Retail));
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
            Items.Clear();
            UrgentItems.Clear();
            await LoadMoreItems();
        }

        [RelayCommand]
        public async Task LoadMoreItems()
        {
            if (Page > TotalPages) { return; }
            List<FilterType> filters = new List<FilterType>();
            foreach(var filter in ActiveFilters)
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
            if (result == null) { return; }
            TotalPages = result.TotalPages;
            ItemNumber = result.TotalCount;
            foreach (var item in result.Items)
            {
                Items.Add(new ListItem(item));
            }
            Page++;
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

            await AttachDetailsView();
        }

        [RelayCommand]
        public async Task AttachDetailsView()
        {
            if (SelectedItem == null) { return; }
            var entity = SelectedItem.Entity;

            ActiveDetailView = entity switch
            {
                //TODO Add the content views for every entity
                Customers => new ContentView(),
                _ => throw new NotImplementedException(),
            };
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
            await SelectItem(item);
            //await ItemDeleteMethod();
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

    }
}
