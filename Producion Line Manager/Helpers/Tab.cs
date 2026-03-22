

using CommunityToolkit.Mvvm.ComponentModel;
using Models.Production;
using System.Collections.ObjectModel;

namespace Producion_Line_Manager.ViewModels
{
    public partial class Tab : ObservableObject
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
        private ObservableCollection<Tab> _children = new();

        [ObservableProperty]
        private ProcessesType? _type;

        public bool HasDetails => !string.IsNullOrEmpty(Details);

        public bool HasChildren => Children.Count > 0;

        [ObservableProperty]
        private bool _isSubTab = false;

        [ObservableProperty]
        private bool _isExpanded = false;

        [ObservableProperty]
        private bool _isSeparator = false;

        public Tab(int id, string name, string? details, ProcessesType? type)
        {
            Id = id;
            Name = name;
            Details = details;
            Type = type;
        }

        public Tab()
        {
            IsSeparator = true;
        }

        public void AddChild(Tab child)
        {
            Children.Add(child);
            OnPropertyChanged(nameof(HasChildren));
        }

        public static Tab FromProcess(Processes process)
        {
            switch (process.Type)
            {
                case ProcessesType.Customers:
                    return new Tab(process.Id, "Customers", null, process.Type);
                case ProcessesType.Orders:
                    return new Tab(process.Id, "Orders", null, process.Type);
                case ProcessesType.Products:
                    return new Tab(process.Id, "Products", null, process.Type);
                case ProcessesType.Users:
                    return new Tab(process.Id, "Users", null, process.Type);
                case ProcessesType.Models:
                    return new Tab(process.Id, "Models", null, process.Type);
                case ProcessesType.Patterns:
                    return new Tab(process.Id, "Patterns", null, process.Type);
                case ProcessesType.ProductCategories:
                    return new Tab(process.Id, "Product Categories", null, process.Type);
                case ProcessesType.PickUpApt:
                    return new Tab(process.Id, "PickUp", null, process.Type);
                case ProcessesType.FoamFix:
                    return new Tab(process.Id, "Foam Fix", null, process.Type);
                case ProcessesType.FoamAdapt:
                    return new Tab(process.Id, "Foam Adapt", null, process.Type);
                case ProcessesType.FoamGel:
                    return new Tab(process.Id, "Foam Gel", null, process.Type);
                case ProcessesType.FoamAnatomical:
                    return new Tab(process.Id, "Foam Anatomical", null, process.Type);
                case ProcessesType.CoverRemove:
                    return new Tab(process.Id, "Remove Cover", null, process.Type);
                case ProcessesType.CustomPattern:
                    return new Tab(process.Id, "Create Custom Pattern", null, process.Type);
                case ProcessesType.Cut:
                    return new Tab(process.Id, "Cut", null, process.Type);
                case ProcessesType.Sew:
                    return new Tab(process.Id, "Sew", null, process.Type);
                case ProcessesType.Embroider:
                    return new Tab(process.Id, "Embroider", null, process.Type);
                case ProcessesType.Bolt:
                    return new Tab(process.Id, "Bolt", null, process.Type);
                case ProcessesType.Inspect:
                    return new Tab(process.Id, "Inspect", null, process.Type);
                case ProcessesType.DeliverApt:
                    return new Tab(process.Id, "Deliver", null, process.Type);
                default:
                    return new Tab(process.Id, "Unknown", null, process.Type);
            }
        }
    }
}
