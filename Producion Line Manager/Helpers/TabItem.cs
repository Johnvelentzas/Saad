

using CommunityToolkit.Mvvm.ComponentModel;
using Models.Production;
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
            switch (process.Type)
            {
                case ProcessesType.Customers:
                    return new TabItem(process.Id, "Customers", null, process.Type);
                case ProcessesType.Orders:
                    return new TabItem(process.Id, "Orders", null, process.Type);
                case ProcessesType.Products:
                    return new TabItem(process.Id, "Products", null, process.Type);
                case ProcessesType.Users:
                    return new TabItem(process.Id, "Users", null, process.Type);
                case ProcessesType.Models:
                    return new TabItem(process.Id, "Models", null, process.Type);
                case ProcessesType.Patterns:
                    return new TabItem(process.Id, "Patterns", null, process.Type);
                case ProcessesType.ProductCategories:
                    return new TabItem(process.Id, "Product Categories", null, process.Type);
                case ProcessesType.PickUpApt:
                    return new TabItem(process.Id, "PickUp", null, process.Type);
                case ProcessesType.FoamFix:
                    return new TabItem(process.Id, "Foam Fix", null, process.Type);
                case ProcessesType.FoamAdapt:
                    return new TabItem(process.Id, "Foam Adapt", null, process.Type);
                case ProcessesType.FoamGel:
                    return new TabItem(process.Id, "Foam Gel", null, process.Type);
                case ProcessesType.FoamAnatomical:
                    return new TabItem(process.Id, "Foam Anatomical", null, process.Type);
                case ProcessesType.CoverRemove:
                    return new TabItem(process.Id, "Remove Cover", null, process.Type);
                case ProcessesType.CustomPattern:
                    return new TabItem(process.Id, "Create Custom Pattern", null, process.Type);
                case ProcessesType.Cut:
                    return new TabItem(process.Id, "Cut", null, process.Type);
                case ProcessesType.Sew:
                    return new TabItem(process.Id, "Sew", null, process.Type);
                case ProcessesType.Embroider:
                    return new TabItem(process.Id, "Embroider", null, process.Type);
                case ProcessesType.Bolt:
                    return new TabItem(process.Id, "Bolt", null, process.Type);
                case ProcessesType.Inspect:
                    return new TabItem(process.Id, "Inspect", null, process.Type);
                case ProcessesType.DeliverApt:
                    return new TabItem(process.Id, "Deliver", null, process.Type);
                case ProcessesType.Tasks:
                    return new TabItem(process.Id, "Tasks", null, process.Type);
                case ProcessesType.Foam:
                    return new TabItem(process.Id, "Foam", null, process.Type);
                default:
                    return new TabItem(process.Id, "Unknown", null, process.Type);
            }
        }
    }
}
