using CommunityToolkit.Mvvm.ComponentModel;
using Models.Attributes;
using Producion_Line_Manager.Views.DetailsViews;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class ProductCategoriesViewModel : BaseEntityViewModel
    {


        [ObservableProperty]
        private string _CategoryName = String.Empty;

        public ProductCategoriesViewModel()
        {
        }

        partial void OnCategoryNameChanged(String value)
        {
            if (Item is ProductCategories type)
            {
                type.CategoryName = value;
                TriggerDebouncedSave();
            }
        }

        public async void LoadEntity(ProductCategories category)
        {
            base.LoadEntity(category);
            CategoryName = category.CategoryName;
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is ProductCategories category)
            {
                category.CategoryName = CategoryName;
            }
        }

    }
}

