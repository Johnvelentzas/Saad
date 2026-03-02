using CommunityToolkit.Mvvm.Input;
using Task_Manager.Models;

namespace Task_Manager.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}