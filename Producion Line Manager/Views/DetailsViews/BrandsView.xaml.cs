using Models.Attributes;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class BrandsView : BaseEntityView
{
	private readonly BrandsViewModel _viewModel;
    public BrandsView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<BrandsViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(Brands entity)
    {
        if (BindingContext is BrandsViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type BrandsViewModel.");
        }
    }
}