using Models.Attributes;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class StitchTypesView : BaseEntityView
{
	private readonly StitchTypesViewModel _viewModel;
    public StitchTypesView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<StitchTypesViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(StitchTypes entity)
    {
        if (BindingContext is StitchTypesViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type StitchTypesViewModel.");
        }
    }
}