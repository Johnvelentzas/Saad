using Producion_Line_Manager.Services;
using Producion_Line_Manager.Helpers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Producion_Line_Manager.ViewModels
{
    public partial class TabListViewModel : BaseViewModel
    {

        private readonly RestService restService;

        [ObservableProperty]
        private ObservableCollection<ListItem> _items;

        [ObservableProperty]
        private ListItem? _selectedItem;

        [ObservableProperty]
        private Tab _currentTab;

        [ObservableProperty]
        private ContentView _activeDetailView;


        public TabListViewModel()
        {
            Title = "List View";
            restService = ServiceHelper.GetService<RestService>();
            Items = new ObservableCollection<ListItem>();
        }

        [RelayCommand]
        public async Task LoadItems(Tab tab)
        {
            if (tab == null)
            {
                return;
            }

        }


        
    }
}
