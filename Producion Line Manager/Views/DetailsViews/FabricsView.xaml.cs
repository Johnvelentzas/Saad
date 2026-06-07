using Models.Attributes;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class FabricsView : BaseEntityView
{
	private readonly FabricsViewModel _viewModel;
    public FabricsView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<FabricsViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(Fabrics entity)
    {
        if (BindingContext is FabricsViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type FabricsViewModel.");
        }
    }
}