using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Services;
using System.Collections.ObjectModel;

namespace Producion_Line_Manager.ViewModels
{
    public partial class UserSelectionViewModel : BaseViewModel
    {

        private readonly RestService restService;

        private readonly NavigationService navigationService;

        [ObservableProperty]
        private ObservableCollection<Users> _users;

        public UserSelectionViewModel()
        {
            Title = "User Selection";
            restService = ServiceHelper.GetService<RestService>();
            navigationService = ServiceHelper.GetService<NavigationService>();
            Users = new ObservableCollection<Users>();
        }

        [RelayCommand]
        public async Task FetchUsers()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Users.Clear();
                var data = await restService.GetUsers();
                Users = new ObservableCollection<Users>(data);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., show an error message)
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SelectUser(Users user)
        {
            if (user == null) return;
            // Save the selected user to preferences
            Preferences.Default.Set("UserId", user.Id.ToString());
            // Navigate back to the main navigation page
            await GoBack();
        }

        [RelayCommand]
        public async Task GoBack()
        {
            await navigationService.GoBack();
        }

    }
}
