using Models;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels;

namespace Producion_Line_Manager.Views;

public partial class TabListView : ContentView
{
	
	private readonly TabListViewModel _viewModel;

    public TabListView()
	{
        InitializeComponent();
		_viewModel = ServiceHelper.GetService<TabListViewModel>();
    }
}