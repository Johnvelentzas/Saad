using CommunityToolkit.Mvvm.ComponentModel;
using Models.Attributes;
using Producion_Line_Manager.Views.DetailsViews;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class PatternsViewModel : BaseEntityViewModel
    {


        [ObservableProperty]
        private string _PatternName = String.Empty;

        public PatternsViewModel()
        {
        }

        partial void OnPatternNameChanged(String value)
        {
            if (Item is Patterns pattern)
            {
                pattern.Name = value;
                TriggerDebouncedSave();
            }
        }

        public async void LoadEntity(Patterns pattern)
        {
            base.LoadEntity(pattern);
            PatternName = pattern.Name;
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is Patterns pattern)
            {
                pattern.Name = PatternName;
            }
        }

    }
}

