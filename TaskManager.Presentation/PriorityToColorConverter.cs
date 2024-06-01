using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskManager.Presentation
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int priority = (int)value;
            switch (priority)
            {
                case 0:
                    return new SolidColorBrush(Colors.LightGreen); // Düşük
                case 1:
                    return new SolidColorBrush(Colors.LightYellow); // Orta
                case 2:
                    return new SolidColorBrush(Colors.LightCoral); // Yüksek
                default:
                    return new SolidColorBrush(Colors.White); // Varsayılan
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
