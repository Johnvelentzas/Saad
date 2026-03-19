using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels;

namespace Producion_Line_Manager.Views;

public partial class MainNavigationPage : ContentPage
{

    private readonly MainNavigationViewModel _viewModel;
    public MainNavigationPage()
    {
        InitializeComponent();

        _viewModel = ServiceHelper.GetService<MainNavigationViewModel>();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MainNavigationViewModel vm)
        {
            vm.FetchDataCommand.ExecuteAsync(null);
        }
    }
}