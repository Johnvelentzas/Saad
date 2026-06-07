using Microsoft.Extensions.Logging;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.ViewModels;
using Producion_Line_Manager.ViewModels.DetailsViewModels;
using Producion_Line_Manager.Views;
using Producion_Line_Manager.Views.DetailsViews;
using CommunityToolkit.Maui;

namespace Producion_Line_Manager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("MaterialSymbolsOutlined-Regular.ttf", "Icons");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SFPRODISPLAYBLACKITALIC.OTF", "SFProDisplayBlackItalic");
                    fonts.AddFont("SFPRODISPLAYBOLD.OTF", "SFProDisplayBold");
                    fonts.AddFont("SFPRODISPLAYHEAVYITALIC.OTF", "SFProDisplayHeavyItalic");
                    fonts.AddFont("SFPRODISPLAYLIGHTITALIC.OTF", "SFProDisplayLightItalic");
                    fonts.AddFont("SFPRODISPLAYMEDIUM.OTF", "SFProDisplayMedium");
                    fonts.AddFont("SFPRODISPLAYREGULAR.OTF", "SFProDisplayRegular");
                    fonts.AddFont("SFPRODISPLAYSEMIBOLDITALIC.OTF", "SFProDisplaySemiboldItalic");
                    fonts.AddFont("SFPRODISPLAYTHINITALIC.OTF", "SFProDisplayThinItalic");
                    fonts.AddFont("SFPRODISPLAYULTRALIGHTITALIC.OTF", "SFProDisplayUltralightItalic");
                })
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews();


#if DEBUG
            builder.Logging.AddDebug();
            
#endif
            var app = builder.Build();

            ServiceHelper.Initialize(app.Services);

            return app;
        }

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<RestService>();
            mauiAppBuilder.Services.AddSingleton<NavigationService>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<MainNavigationViewModel>();
            mauiAppBuilder.Services.AddSingleton<SettingsViewModel>();
            mauiAppBuilder.Services.AddSingleton<UserSelectionViewModel>();
            mauiAppBuilder.Services.AddTransient<TabListViewModel>();
            mauiAppBuilder.Services.AddTransient<OverviewViewModel>();

            mauiAppBuilder.Services.AddTransient<CustomersViewModel>();
            mauiAppBuilder.Services.AddTransient<OrdersViewModel>();
            mauiAppBuilder.Services.AddTransient<StitchTypesViewModel>();
            mauiAppBuilder.Services.AddTransient<YarnColorsViewModel>();
            mauiAppBuilder.Services.AddTransient<PatternsViewModel>();
            mauiAppBuilder.Services.AddTransient<FabricsViewModel>();
            mauiAppBuilder.Services.AddTransient<ProductCategoriesViewModel>();
            mauiAppBuilder.Services.AddTransient<BrandsViewModel>();
            mauiAppBuilder.Services.AddTransient<ModelsViewModel>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<MainNavigationPage>();
            mauiAppBuilder.Services.AddSingleton<SettingsPage>();
            mauiAppBuilder.Services.AddSingleton<UserSelectionPage>();
            mauiAppBuilder.Services.AddTransient<TabListView>();
            mauiAppBuilder.Services.AddTransient<OverviewView>();

            mauiAppBuilder.Services.AddTransient<CustomersView>();
            mauiAppBuilder.Services.AddTransient<OrdersView>();
            mauiAppBuilder.Services.AddTransient<StitchTypesView>();
            mauiAppBuilder.Services.AddTransient<YarnColorsView>();
            mauiAppBuilder.Services.AddTransient<PatternsView>();
            mauiAppBuilder.Services.AddTransient<FabricsView>();
            mauiAppBuilder.Services.AddTransient<ProductCategoriesView>();
            mauiAppBuilder.Services.AddTransient<BrandsView>();
            mauiAppBuilder.Services.AddTransient<ModelsView>();

            return mauiAppBuilder;
        }
    }
}
