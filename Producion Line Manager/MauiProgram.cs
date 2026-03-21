using Microsoft.Extensions.Logging;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.ViewModels;
using Producion_Line_Manager.Views;

namespace Producion_Line_Manager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
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

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<MainNavigationViewModel>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<MainNavigationPage>();
            mauiAppBuilder.Services.AddSingleton<SettingsPage>();
            mauiAppBuilder.Services.AddSingleton<UserSelectionPage>();

            return mauiAppBuilder;
        }
    }
}
