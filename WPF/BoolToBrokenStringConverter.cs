using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF;

public class BoolToBrokenStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isBroken)
        {
            return isBroken ? "Broken" : "Not Broken";
        }

        return "Unknown";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
