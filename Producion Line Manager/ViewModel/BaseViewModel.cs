using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Producion_Line_Manager.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {
        public BaseViewModel()
        {
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        string title = "Page";

        [ObservableProperty]
        bool isBusy = false;

        public bool IsNotBusy => !IsBusy;
    }
}
