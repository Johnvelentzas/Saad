using Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Producion_Line_Manager.Helpers
{
    public partial class EntitySortItem : ObservableObject
    {
        [ObservableProperty]
        private string _name = "Newest";

        [ObservableProperty]
        private Boolean _isSelected = false;

        [ObservableProperty]
        private SortType _type = SortType.IdDecending;

        [ObservableProperty]
        private string _image = String.Empty;

        public EntitySortItem()
        {
        }
        public EntitySortItem(SortType type)
        {
            switch (type)
            {
                case SortType.IdAccending:
                    Name = "Oldest";
                    Type = type;
                    break;
                case SortType.IdDecending:
                    Name = "Newest";
                    Type = type;
                    break;

                default:
                    break;

            }
        }
    }
}