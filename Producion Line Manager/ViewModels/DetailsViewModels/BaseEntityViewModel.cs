
using CommunityToolkit.Mvvm.ComponentModel;
using Models;
using Producion_Line_Manager.ViewModels;

namespace Producion_Line_Manager.Views.DetailsViews
{
    public partial class BaseEntityViewModel : BaseViewModel
    {
        [ObservableProperty]
        private IEntity? _item;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotDraft))]
        private bool _isDraft = false;

        public bool IsNotDraft => !IsDraft;

        [ObservableProperty]
        private int _Id = 0;

        public void LoadEntity(IEntity entity)
        {
            Item = entity;
            IsDraft = entity.IsDraft;
            Id = entity.Id;
        }

        public virtual void SaveEntity()
        {
            if (Item == null) { return; }
            Item.Id = Id;
            Item.IsDraft = IsDraft;
        }
    }
}
