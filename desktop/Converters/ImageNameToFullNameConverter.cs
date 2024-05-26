using Avalonia.Data.Converters;
using desktop.Views;
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Converters
{
    public class ImageNameToFullNameConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string nameImage = value as string;
            nameImage = nameImage.Replace("\\", "/");
            if (nameImage != null)
            {
                return SplashWindow.RestApiURL + "/Image/" + nameImage;
            }
            else return "";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
