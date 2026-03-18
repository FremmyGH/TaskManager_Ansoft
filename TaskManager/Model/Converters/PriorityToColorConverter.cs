using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskManager.Model.Converters
{
    /// <summary>
    /// Конвертер цвета приоритета
    /// </summary>
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            (TodoPriority)v switch
            {
                TodoPriority.High => Brushes.Red,
                TodoPriority.Medium => Brushes.Orange,
                _ => Brushes.Green
            };
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
}
