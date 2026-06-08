using Models.Management;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class UsersView : BaseEntityView
{
	private readonly UsersViewModel _viewModel;
    public UsersView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<UsersViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(Users entity)
    {
        if (BindingContext is UsersViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type UsersViewModel.");
        }
    }
}