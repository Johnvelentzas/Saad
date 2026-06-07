using Models.Attributes;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class YarnColorsView : BaseEntityView
{
	private readonly YarnColorsViewModel _viewModel;
    public YarnColorsView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<YarnColorsViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(YarnColors entity)
    {
        if (BindingContext is YarnColorsViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type YarnColorsViewModel.");
        }
    }
}