using Models.Attributes;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class ModelsView : BaseEntityView
{
	private readonly ModelsViewModel _viewModel;
    public ModelsView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<ModelsViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(Models.Attributes.Models entity)
    {
        if (BindingContext is ModelsViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type ModelsViewModel.");
        }
    }
}