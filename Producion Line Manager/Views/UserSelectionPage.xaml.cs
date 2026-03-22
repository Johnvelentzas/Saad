using Producion_Line_Manager.ViewModels;
using Producion_Line_Manager.Helpers;

namespace Producion_Line_Manager.Views;

public partial class UserSelectionPage : ContentPage
{

    private readonly UserSelectionViewModel _viewModel;
    public UserSelectionPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetService<UserSelectionViewModel>();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is UserSelectionViewModel vm)
        {
            vm.FetchUsersCommand.ExecuteAsync(null);
        }
    }
}