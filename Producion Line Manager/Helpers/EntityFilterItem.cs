
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

        public List<FilterType> Incompatible = new();

        [ObservableProperty]
        private string _image = String.Empty;

        public EntityFilterItem(FilterType type)
        {
            Type = type;
            switch (type)
            {
                case FilterType.Active:
                    Name = "Active";
                    break;
                case FilterType.Complete:
                    Name = "Complete";
                    break;
                case FilterType.Pending:
                    Name = "Pending";
                    break;
                case FilterType.Draft:
                    Name = "Drafts";
                    break;
                case FilterType.Urgent:
                    Name = "Urgent";
                    break;
                case FilterType.Deleted:
                    Name = "Deleted";
                    break;
                case FilterType.None:
                    Name = "All";
                    break;
                case FilterType.Incomplete:
                    Name = "Incomplete";
                    break;
                case FilterType.HasComments:
                    Name = "Has Comments";
                    break;
                case FilterType.Retail:
                    Name = "Retail";
                    Incompatible.Add(FilterType.Wholesale);
                    break;
                case FilterType.Wholesale:
                    Name = "Wholesale";
                    Incompatible.Add(FilterType.Retail);
                    break;
                case FilterType.InStore:
                    Name = "In Store";
                    break;
                case FilterType.Phone:
                    Name = "Phone";
                    break;
                case FilterType.Online:
                    Name = "Online";
                    break;
                case FilterType.Email:
                    Name = "Email";
                    break;
                case FilterType.SocialMedia:
                    Name = "Social Media";
                    break;
                default:
                    break;

            }
        }
    }
}
