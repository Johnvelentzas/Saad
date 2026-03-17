

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Models.Production;

namespace Producion_Line_Manager.ViewModel
{
    public partial class FlyoutViewModel : BaseViewModel
    {
        [ObservableProperty]
        Users user;

        [ObservableProperty]
        ObservableCollection<Processes> processes;
    }
}
