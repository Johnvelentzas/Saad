

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Producion_Line_Manager.ViewModels
{
    public partial class MainNavigationViewModel : BaseViewModel
    {

        private readonly RestService restService;

        [ObservableProperty]
        private Users _user;

        [ObservableProperty]
        private ObservableCollection<Processes> _processes;

        public MainNavigationViewModel()
        {
            User = Preferences.Default.Get("User", new Users() { Id = 1, Name = "Admin" });
            Processes = new ObservableCollection<Processes>();
            restService = ServiceHelper.GetService<RestService>();
        }

        [RelayCommand]
        public async Task FetchData()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var data = await restService.GetUserProcesses(User);

                Processes.Clear();
                foreach (var process in data)
                {
                    Processes.Add(process);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching processes: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
