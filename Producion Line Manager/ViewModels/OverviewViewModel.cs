using Producion_Line_Manager.Services;
using Producion_Line_Manager.Helpers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Models;

namespace Producion_Line_Manager.ViewModels
{
    public partial class OverviewViewModel : BaseViewModel
    {

        private readonly RestService restService;



        public OverviewViewModel()
        {
            Title = "Overview";
            restService = ServiceHelper.GetService<RestService>();
        }
    }
}
