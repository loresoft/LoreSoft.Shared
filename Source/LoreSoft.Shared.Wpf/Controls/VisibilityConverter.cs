using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LoreSoft.Shared.Controls
{
  /// <summary>
  /// A type converter for visibility and boolean values.
  /// </summary>
  public class VisibilityConverter : IValueConverter
  {
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
      if (value == null)
        return null;

      bool visibility = (bool)value;

      // inverse if parameter is true
      bool b;
      if (parameter != null && bool.TryParse(parameter.ToString(), out b) && b)
        visibility = !visibility;

      return visibility ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
      Visibility visibility = (Visibility)value;
      return (visibility == Visibility.Visible);
    }
  }
}
