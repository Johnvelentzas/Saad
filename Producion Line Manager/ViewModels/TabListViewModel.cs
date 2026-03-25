using Producion_Line_Manager.Services;
using Producion_Line_Manager.Helpers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Finances;

namespace Producion_Line_Manager.ViewModels
{
    public partial class TabListViewModel : BaseViewModel
    {

        private readonly RestService restService;

        [ObservableProperty]
        private ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();

        [ObservableProperty]
        private int _itemNumber = 0;

        [ObservableProperty]
        private ListItem? _selectedItem;

        [ObservableProperty]
        private TabItem _currentTab;

        [ObservableProperty]
        private ContentView _activeDetailView;


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
            await LoadItems();
            await SelectItemIndex(0);
        }

        [RelayCommand]
        public async Task LoadItems()
        {
            if (CurrentTab == null) { return; }
            switch (CurrentTab.Type)
            {
                case Models.Production.ProcessesType.Customers:
                    Title = "Customers";
                    List<Customers> customers = await restService.GetCustomers();
                    foreach(var cus in customers)
                    {
                        Items.Add(new ListItem(cus));
                    }
                    break;
                default:
                    break;
            }
            ItemNumber = Items.Count();
        }

        [RelayCommand]
        public async Task SelectItem(ListItem item)
        {

        }

        [RelayCommand]
        public async Task SelectItemIndex(int  index)
        {

        }

        [RelayCommand]
        public async Task DeleteItem(ListItem item)
        {
            if (item == null) { return; }
        }
    }
}
