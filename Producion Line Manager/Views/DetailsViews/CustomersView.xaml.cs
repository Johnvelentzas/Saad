using Models.Finances;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class CustomersView : BaseEntityView
{
	private readonly CustomersViewModel _viewModel;
    public CustomersView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<CustomersViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(Customers entity)
    {
        if (BindingContext is CustomersViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type CustomersViewModel.");
        }
    }
}