
using Producion_Line_Manager.Helpers;

namespace Producion_Line_Manager.Services
{
    public class NavigationService
    {

        public NavigationService()
        {
        }

        public async Task NavigateTo<T>() where T : Page
        {
            var navigation = Application.Current?.Windows[0]?.Page?.Navigation;
            var page = ServiceHelper.GetService<T>();
            if (navigation != null && page != null)
            {
                await navigation.PushAsync(page);
            }
        }

        public async Task GoBack()
        {
            var navigation = Application.Current?.Windows[0]?.Page?.Navigation;
            if (navigation != null && navigation.NavigationStack.Count > 1)
            {
                await navigation.PopAsync();
            }
        }
    }
}
