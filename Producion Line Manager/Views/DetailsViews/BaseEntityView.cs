using Models;

namespace Producion_Line_Manager.Views.DetailsViews
{
    public class BaseEntityView : ContentView
    {
        public void LoadEntity(IEntity entity)
        {
            if (BindingContext is BaseEntityViewModel viewModel)
            {
                viewModel.LoadEntity((dynamic)entity);
            }
        }

        public void SaveEntity()
        {
            if (BindingContext is BaseEntityViewModel viewModel)
            {
                viewModel.SaveEntity();
            }
        }
    }
}
