using CommunityToolkit.Mvvm.ComponentModel;
using Models.Attributes;
using Producion_Line_Manager.Views.DetailsViews;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class StitchTypesViewModel : BaseEntityViewModel
    {


        [ObservableProperty]
        private string _StitchTypeName = String.Empty;

        public StitchTypesViewModel()
        {
        }

        partial void OnStitchTypeNameChanged(String value)
        {
            if (Item is StitchTypes type)
            {
                type.StitchTypeName = value;
                TriggerDebouncedSave();
            }
        }

        public async void LoadEntity(StitchTypes stitchType)
        {
            base.LoadEntity(stitchType);
            StitchTypeName = stitchType.StitchTypeName;
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is StitchTypes stitchType)
            {
                stitchType.StitchTypeName = StitchTypeName;
            }
        }

    }
}

