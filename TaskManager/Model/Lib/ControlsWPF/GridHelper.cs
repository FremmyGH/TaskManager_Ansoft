using System.Windows;
using System.Windows.Controls;

namespace TaskManager.Model.Lib.ControlsWPF
{
    public static class GridHelper
    {
        private static readonly GridLengthConverter _converter = new();

        #region Definitions (Binding support)

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached("Rows", typeof(string), typeof(GridHelper), 
                new PropertyMetadata(null, OnRowsChanged));

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached("Columns", typeof(string), typeof(GridHelper), 
                new PropertyMetadata(null, OnColumnsChanged));

        public static string GetRows(DependencyObject obj) => (string)obj.GetValue(RowsProperty);
        public static void SetRows(DependencyObject obj, string value) => obj.SetValue(RowsProperty, value);

        public static string GetColumns(DependencyObject obj) => (string)obj.GetValue(ColumnsProperty);
        public static void SetColumns(DependencyObject obj, string value) => obj.SetValue(ColumnsProperty, value);

        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid grid || e.NewValue is not string def) return;

            grid.RowDefinitions.Clear();
            foreach (var length in Parse(def))
                grid.RowDefinitions.Add(new RowDefinition { Height = length });
        }

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid grid || e.NewValue is not string def) return;

            grid.ColumnDefinitions.Clear();
            foreach (var length in Parse(def))
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = length });
        }
        #endregion

        #region RC (Quick Placement: Row Column [RowSpan] [ColSpan])

        public static readonly DependencyProperty RCProperty =
            DependencyProperty.RegisterAttached("RC", typeof(string), typeof(GridHelper), 
                new PropertyMetadata(null, OnRCChanged));

        public static string GetRC(DependencyObject obj) => (string)obj.GetValue(RCProperty);
        public static void SetRC(DependencyObject obj, string value) => obj.SetValue(RCProperty, value);

        private static void OnRCChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element || e.NewValue is not string rc) return;

            var parts = rc.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return;

            if (int.TryParse(parts[0], out int r)) Grid.SetRow(element, r);
            if (int.TryParse(parts[1], out int c)) Grid.SetColumn(element, c);

            if (parts.Length >= 4)
            {
                if (int.TryParse(parts[2], out int rs)) Grid.SetRowSpan(element, rs);
                if (int.TryParse(parts[3], out int cs)) Grid.SetColumnSpan(element, cs);
            }
        }
        #endregion

        private static IEnumerable<GridLength> Parse(string def)
        {
            return def.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries)
                      .Select(p => 
                      {
                          var val = p.Equals("a", StringComparison.OrdinalIgnoreCase) ? "Auto" : p;
                          return (GridLength)(_converter.ConvertFromString(val) ?? GridLength.Auto);
                      });
        }
    }
}
