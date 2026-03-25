

using CommunityToolkit.Mvvm.ComponentModel;
using Models;
using Models.Finances;

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

        [ObservableProperty]
        private bool _isSelected;

        public ListItem(int id, string name, string description, IEntity entity)
        {
            Id = id;
            Name = name;
            Description = description;
            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Customers customer)
        {
            Id = customer.Id;
            Name = $"{customer.FirstName} {customer.LastName}";
            if(customer.TaxNumber != null)
            {
                Description = customer.TaxNumber.ToString();
            }else if(customer.Telephone != null)
            {
                Description = customer.Telephone.ToString();
            }else if(customer.Email != null)
            {
                Description = customer.Email.ToString();
            }
            Entity = customer;
            IsSelected = false;
        }
    }
}
