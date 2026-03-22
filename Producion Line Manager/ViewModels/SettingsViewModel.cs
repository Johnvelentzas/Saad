using CommunityToolkit.Mvvm.ComponentModel;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Services;
using System.Collections.ObjectModel;

namespace Producion_Line_Manager.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {

        private readonly RestService restService;

        public SettingsViewModel()
        {
            Title = "Settings";
            restService = ServiceHelper.GetService<RestService>();
        }
    }
}
