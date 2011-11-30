using System;
using System.Windows.Data;

namespace LoreSoft.Shared.Controls
{
  public class EnumBooleanConverter: IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null || parameter == null)
        return value;

      return string.Equals(
        value.ToString(), 
        parameter.ToString(), 
        StringComparison.CurrentCultureIgnoreCase);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null || parameter == null)
        return value;

      return Enum.Parse(targetType, parameter.ToString(), true);
    }
  }
}
