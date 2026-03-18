using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskManager.Model.Converters
{
    /// <summary>
    /// Конвертер для проверки дедлайна и окраса строки
    /// </summary>
    public class OverdueToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOverdue && isOverdue)
                return new SolidColorBrush(Color.FromRgb(255, 230, 230));
            
            return Brushes.Transparent;
        }

        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
}
