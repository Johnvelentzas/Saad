
using CommunityToolkit.Mvvm.ComponentModel;
using Models;
using Producion_Line_Manager.ViewModels;

namespace Producion_Line_Manager.Views.DetailsViews
{
    public partial class BaseEntityViewModel : BaseViewModel
    {
        [ObservableProperty]
        private IEntity _entity;

        [ObservableProperty]
        private bool _isDraft;

        public async Task LoadEntity(IEntity entity)
        {
            Entity = entity;
            IsDraft = entity.IsDraft;
        }
    }
}
