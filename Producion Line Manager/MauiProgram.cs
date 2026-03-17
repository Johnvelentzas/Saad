using Microsoft.Extensions.Logging;

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
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
