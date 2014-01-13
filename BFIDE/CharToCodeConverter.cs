namespace BFIDE
{
    using System.Windows.Data;

    public class CharToCodeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString().Length > 0)
            {
                return (int)value.ToString()[0];
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
