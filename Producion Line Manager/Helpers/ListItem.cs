

using CommunityToolkit.Mvvm.ComponentModel;
using Models;

namespace Producion_Line_Manager.Helpers
{
    public partial class ListItem : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        private IEntity _entity;

        public ListItem(int id, string name, string description, IEntity entity)
        {
            Id = id;
            Name = name;
            Description = description;
            Entity = entity;
        }
    }
}
