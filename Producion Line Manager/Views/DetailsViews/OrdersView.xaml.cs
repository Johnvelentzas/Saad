using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class OrdersView : BaseEntityView
{
	private readonly OrdersViewModel _viewModel;
    public OrdersView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<OrdersViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(Orders entity)
    {
        if (BindingContext is OrdersViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type OrdersViewModel.");
        }
    }
}