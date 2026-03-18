using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace TaskManager.Model.Converters
{
    /// <summary>
    /// Конвертер описания enum
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Enum enumValue)
                return string.Empty;

            string? name = enumValue.ToString();
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            //Извлекаем поле через рефлексию
            FieldInfo? fi = value.GetType().GetField(name);
            if (fi == null)
                return name;

            //Ищем атрибут Description
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
