

using CommunityToolkit.Mvvm.ComponentModel;
using Models;
using Models.Production;
using Models.Attributes;
using Models.Management;

namespace Producion_Line_Manager.Helpers
{
    public partial class ListItem : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _imageUrl = String.Empty;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        private IEntity _entity;

        [ObservableProperty]
        private bool _isSelected;

        public ListItem(int id, string name, string imageUrl, string description, IEntity entity)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
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
            if(!String.IsNullOrEmpty(entity.TaxNumber))
            {
                Description = entity.TaxNumber.ToString();
            }else if(!String.IsNullOrEmpty(entity.Telephone))
            {
                Description = entity.Telephone.ToString();
            }else if(!String.IsNullOrEmpty(entity.Email))
            {
                Description = entity.Email.ToString();
            }
            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Orders entity)
        {
            Id = entity.Id;
            Name = $"Order #O-{entity.Id}";

            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Products entity)
        {
            Id = entity.Id;
            Name = $"Product #P-{entity.Id}";

            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Models.Attributes.Models entity)
        {
            Id = entity.Id;
            Name = entity.ModelName;
            ImageUrl = entity.ImageUrl ?? String.Empty;
            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Patterns entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            ImageUrl = entity.ImageUrl ?? String.Empty;
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

        public ListItem(StitchTypes entity)
        {
            Id = entity.Id;
            Name = entity.StitchTypeName;

            Entity = entity;
            IsSelected = false;
        }

        public ListItem(YarnColors entity)
        {
            Id = entity.Id;
            Name = entity.YarnColorName;

            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Fabrics entity)
        {
            Id = entity.Id;
            Name = entity.FabricName;
            ImageUrl = entity.ImageUrl ?? String.Empty;
            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Brands entity)
        {
            Id = entity.Id;
            Name = entity.BrandName;
            ImageUrl = entity.ImageUrl ?? String.Empty;
            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Users entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            ImageUrl = entity.ImageUrl ?? String.Empty;
            Entity = entity;
            IsSelected = false;
        }

        public ListItem(Tasks entity, string process)
        {
            Id = entity.Id;
            Name = $"Product #P-{entity.ProductId}";
            Description = process;

            Entity = entity;
            IsSelected = false;
        }

    }
}
