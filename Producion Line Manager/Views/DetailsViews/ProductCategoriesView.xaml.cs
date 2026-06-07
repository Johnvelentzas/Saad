using Models.Attributes;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class ProductCategoriesView : BaseEntityView
{
	private readonly ProductCategoriesViewModel _viewModel;
    public ProductCategoriesView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<ProductCategoriesViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(ProductCategories entity)
    {
        if (BindingContext is ProductCategoriesViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type ProductCategoriesViewModel.");
        }
    }
}