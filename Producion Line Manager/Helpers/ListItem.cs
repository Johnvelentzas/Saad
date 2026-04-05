

using CommunityToolkit.Mvvm.ComponentModel;
using Models;
using Models.Finances;
using Models.Production;
using Models.Attributes;

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

        
        public ListItem(IEntity entity)
        {
            Id = entity.Id;
            Name = $"#{entity.Id}";
            Description = "Entity";
            Entity = entity;
            IsSelected = false;
        }
        

        public ListItem(Customers entity)
        {
            Id = entity.Id;
            Name = $"{entity.FirstName} {entity.LastName}";
            if(entity.TaxNumber != null)
            {
                Description = entity.TaxNumber.ToString();
            }else if(entity.Telephone != null)
            {
                Description = entity.Telephone.ToString();
            }else if(entity.Email != null)
            {
                Description = entity.Email.ToString();
            }
            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Orders entity)
        {
            Id = entity.Id;
            Name = $"#{entity.Id}";

            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Products entity)
        {
            Id = entity.Id;
            Name = $"#{entity.Id}";

            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Models.Attributes.Models entity)
        {
            Id = entity.Id;
            Name = $"#{entity.Id}";

            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Patterns entity)
        {
            Id = entity.Id;
            Name = $"#{entity.Id}";

            Entity = entity;
            IsSelected = false;
        }

        public ListItem(ProductCategories entity)
        {
            Id = entity.Id;
            Name = $"{entity.CategoryName}";

            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Tasks entity)
        {
            Id = entity.Id;
            Name = $"#{entity.Id}";

            Entity = entity;
            IsSelected = false;
        }

    }
}
