

using CommunityToolkit.Mvvm.ComponentModel;
using Models;

namespace Producion_Line_Manager.Helpers
{
    public partial class ListItem<T> : ObservableObject where T : IEntity
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        private IEntity entity;

        public ListItem()
        {

        }
    }
}
