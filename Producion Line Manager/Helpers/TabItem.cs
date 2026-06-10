

using CommunityToolkit.Mvvm.ComponentModel;
using Models.Management;
using System.Collections.ObjectModel;

namespace Producion_Line_Manager.Helpers
{
    public partial class TabItem : ObservableObject
    {
        [ObservableProperty]
        private int _id = 0;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _iconText = string.Empty;

        [ObservableProperty]
        private string _iconColor = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasDetails))]
        private string? _details;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasChildren))]
        private ObservableCollection<TabItem> _children = new();

        [ObservableProperty]
        private ProcessesType _type;

        public bool HasDetails => !string.IsNullOrEmpty(Details);

        public bool HasChildren => Children.Count > 0;

        [ObservableProperty]
        private bool _isSubTab = false;

        [ObservableProperty]
        private bool _isExpanded = false;

        [ObservableProperty]
        private bool _isSeparator = false;

        [ObservableProperty]
        private bool _isActive = false;

        public TabItem(int id, string name, string? details, ProcessesType type)
        {
            Id = id;
            Name = name;
            Details = details;
            Type = type;
        }

        public TabItem()
        {
            IsSeparator = true;
        }

        public void AddChild(TabItem child)
        {
            Children.Add(child);
            OnPropertyChanged(nameof(HasChildren));
        }

        public static TabItem FromProcess(Processes process)
        {
            TabItem tabItem;
            switch (process.Type)
            {
                case ProcessesType.Customers:
                    tabItem = new TabItem(process.Id, "Customers", null, process.Type);
                    break;
                case ProcessesType.Orders:
                    tabItem = new TabItem(process.Id, "Orders", null, process.Type);
                    break;
                case ProcessesType.Products:
                    tabItem = new TabItem(process.Id, "Products", null, process.Type);
                    break;
                case ProcessesType.Users:
                    tabItem = new TabItem(process.Id, "Users", null, process.Type);
                    break;
                case ProcessesType.Models:
                    tabItem = new TabItem(process.Id, "Models", null, process.Type);
                    break;
                case ProcessesType.Patterns:
                    tabItem = new TabItem(process.Id, "Patterns", null, process.Type);
                    break;
                case ProcessesType.StitchTypes:
                    tabItem = new TabItem(process.Id, "Stitch Types", null, process.Type);
                    break;
                case ProcessesType.YarnColors:
                    tabItem = new TabItem(process.Id, "Yarn Colors", null, process.Type);
                    break;
                case ProcessesType.Fabrics:
                    tabItem = new TabItem(process.Id, "Fabrics", null, process.Type);
                    break;
                case ProcessesType.ProductCategories:
                    tabItem = new TabItem(process.Id, "Product Categories", null, process.Type);
                    break;
                case ProcessesType.DropOffApt:
                    tabItem = new TabItem(process.Id, "Drop Off", null, process.Type);
                    break;
                case ProcessesType.TestTryApt:
                    tabItem = new TabItem(process.Id, "Test Try", null, process.Type);
                    break;
                case ProcessesType.PickUpApt:
                    tabItem = new TabItem(process.Id, "Pick Up", null, process.Type);
                    break;
                case ProcessesType.FoamFix:
                    tabItem = new TabItem(process.Id, "Foam Fix", null, process.Type);
                    break;
                case ProcessesType.FoamAdapt:
                    tabItem = new TabItem(process.Id, "Foam Adapt", null, process.Type);
                    break;
                case ProcessesType.FoamGel:
                    tabItem = new TabItem(process.Id, "Foam Gel", null, process.Type);
                    break;
                case ProcessesType.FoamAnatomical:
                    tabItem = new TabItem(process.Id, "Foam Anatomical", null, process.Type);
                    break;
                case ProcessesType.CoverRemove:
                    tabItem = new TabItem(process.Id, "Remove Cover", null, process.Type);
                    break;
                case ProcessesType.CustomPattern:
                    tabItem = new TabItem(process.Id, "Create Custom Pattern", null, process.Type);
                    break;
                case ProcessesType.Cut:
                    tabItem = new TabItem(process.Id, "Cut", null, process.Type);
                    break;
                case ProcessesType.Sew:
                    tabItem = new TabItem(process.Id, "Sew", null, process.Type);
                    break;
                case ProcessesType.Embroider:
                    tabItem = new TabItem(process.Id, "Embroider", null, process.Type);
                    break;
                case ProcessesType.Bolt:
                    tabItem = new TabItem(process.Id, "Bolt", null, process.Type);
                    break;
                case ProcessesType.Inspect:
                    tabItem = new TabItem(process.Id, "Inspect", null, process.Type);
                    break;
                case ProcessesType.Tasks:
                    tabItem = new TabItem(process.Id, "Tasks", null, process.Type);
                    break;
                case ProcessesType.Foam:
                    tabItem = new TabItem(process.Id, "Foam", null, process.Type);
                    break;
                case ProcessesType.Calendar:
                    tabItem = new TabItem(process.Id, "Calendar", null, process.Type);
                    break;
                case ProcessesType.Brands:
                    tabItem = new TabItem(process.Id, "Brands", null, process.Type);
                    break;
                default:
                    tabItem = new TabItem(process.Id, "Unknown", null, process.Type);
                    break;
            }
            tabItem.IconColor = process.Color;
            tabItem.IconText = process.IconText;
            return tabItem;
        }
    }
}
