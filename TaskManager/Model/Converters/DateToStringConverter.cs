using System.Globalization;
using System.Windows.Data;

namespace TaskManager.Model.Converters
{
    /// <summary>
    /// Конвертер даты в строку
    /// </summary>
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                if (date.Date == DateTime.Today) return "Сегодня";
                if (date.Date == DateTime.Today.AddDays(1)) return "Завтра";
                if (date.Date == DateTime.Today.AddDays(-1)) return "Вчера";
                return date.ToString("dd.MM.yyyy");
            }
            return "Срок не задан";
        }

        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
}
