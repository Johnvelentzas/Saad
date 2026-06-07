using Models.Attributes;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class PatternsView : BaseEntityView
{
	private readonly PatternsViewModel _viewModel;
    public PatternsView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<PatternsViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(Patterns entity)
    {
        if (BindingContext is PatternsViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type PatternsViewModel.");
        }
    }
}