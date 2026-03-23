using Models;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels;

namespace Producion_Line_Manager.Views;

public partial class OverviewView : ContentView
{
	
	private readonly OverviewViewModel _viewModel;

    public OverviewView()
	{
        InitializeComponent();
		_viewModel = ServiceHelper.GetService<OverviewViewModel>();
    }
}