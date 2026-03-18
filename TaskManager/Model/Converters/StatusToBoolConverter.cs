using System.Globalization;
using System.Windows.Data;

namespace TaskManager.Model.Converters
{
    /// <summary>
    /// Конвертер статуса по значению bool
    /// </summary>
    public class StatusToBoolConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v is TodoStatus status && status == TodoStatus.Completed;

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            (bool)v ? TodoStatus.Completed : TodoStatus.Active;
    }
}
