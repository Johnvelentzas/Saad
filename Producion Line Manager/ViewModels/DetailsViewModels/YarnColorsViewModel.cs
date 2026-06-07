using CommunityToolkit.Mvvm.ComponentModel;
using Models.Attributes;
using Producion_Line_Manager.Views.DetailsViews;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class YarnColorsViewModel : BaseEntityViewModel
    {


        [ObservableProperty]
        private string _YarnColorName = String.Empty;

        public YarnColorsViewModel()
        {
        }

        partial void OnYarnColorNameChanged(String value)
        {
            if (Item is YarnColors color)
            {
                color.YarnColorName = value;
                TriggerDebouncedSave();
            }
        }

        public async void LoadEntity(YarnColors yarnColor)
        {
            base.LoadEntity(yarnColor);
            YarnColorName = yarnColor.YarnColorName;
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is YarnColors yarnColor)
            {
                yarnColor.YarnColorName = YarnColorName;
            }
        }

    }
}

