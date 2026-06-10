using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.ViewModels.DetailsViewModels;

namespace Producion_Line_Manager.Views.DetailsViews;

public partial class TasksView : BaseEntityView
{
	private readonly TasksViewModel _viewModel;
    public TasksView()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<TasksViewModel>();
        BindingContext = _viewModel;
    }

    public void LoadEntity(Tasks entity)
    {
        if (BindingContext is TasksViewModel viewModel)
        {
            viewModel.LoadEntity((dynamic)entity);
        }
        else
        {
            throw new InvalidOperationException("BindingContext is not of type TasksViewModel.");
        }
    }
}