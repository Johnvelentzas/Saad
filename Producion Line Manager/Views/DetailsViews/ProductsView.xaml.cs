using Models.Attributes;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class ProductsView : BaseEntityView
{
	private readonly ProductsViewModel _viewModel;
    public ProductsView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<ProductsViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(Products entity)
    {
        if (BindingContext is ProductsViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type ProductsViewModel.");
        }
    }
}