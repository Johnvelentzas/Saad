using System.Globalization;
using Microsoft.Maui.Graphics;

namespace Producion_Line_Manager.Converters
{
    public class HexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hex && !string.IsNullOrWhiteSpace(hex))
            {
                return Color.FromArgb(hex);
            }
            return Colors.Gray; // Fallback color if the DB value is missing
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}