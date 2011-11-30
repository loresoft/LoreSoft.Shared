using System;
using System.Windows;
using System.Windows.Data;

namespace LoreSoft.Shared.Controls
{
  public class EnumVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null || parameter == null)
        return value;

      bool isVisible = string.Equals(
        value.ToString(),
        parameter.ToString(),
        StringComparison.CurrentCultureIgnoreCase);

      return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}