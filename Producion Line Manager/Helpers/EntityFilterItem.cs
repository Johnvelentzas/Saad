
using Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Producion_Line_Manager.Helpers
{
    public partial class EntityFilterItem : ObservableObject
    {
        [ObservableProperty]
        private string _name = "All";

        [ObservableProperty]
        private Boolean _isSelected = false;

        [ObservableProperty]
        private FilterType _type = FilterType.None;

        [ObservableProperty]
        private string _image = String.Empty;

        public EntityFilterItem(FilterType type)
        {
            switch (type)
            {
                case FilterType.Active:
                    Name = "Active";
                    Type = type; 
                    break;
                case FilterType.Complete:
                    Name = "Complete";
                    Type = type;
                    break;
                case FilterType.Pending:
                    Name = "Pending";
                    Type = type;
                    break;
                case FilterType.Draft:
                    Name = "Draft";
                    Type = type;
                    break;
                case FilterType.Urgent:
                    Name = "Urgent";
                    Type = type;
                    break;
                case FilterType.Deleted:
                    Name = "Deleted";
                    Type = type;
                    break;
                default:
                    break;

            }
        }
    }
}
