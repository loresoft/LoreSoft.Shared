using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Controls
{
  public class BooleanColorConverter : IValueConverter
  {
    private static readonly Lazy<IDictionary<string, Color>> _knownColors 
      = new Lazy<IDictionary<string, Color>>(GetKnownColors);

    private static Dictionary<string, Color> GetKnownColors()
    {
      var colorProperties = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
      return colorProperties
          .ToDictionary(
              p => p.Name,
              p => (Color)p.GetValue(null, null));
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;

      bool isTrue = (bool)value;
      
      string colors = parameter as string;
      if (colors.IsNullOrEmpty())
        return isTrue 
          ? new SolidColorBrush(Colors.Black) 
          : new SolidColorBrush(Colors.Red);

      // first color is true, second color is false
      string[] colorParts = colors.SplitAndTrim(',', ';');

      Color trueColor = Colors.Black;
      Color falseColor = Colors.Red;

      if (colorParts.Length > 0)
        _knownColors.Value.TryGetValue(colorParts[0], out trueColor);
      if (colorParts.Length > 1)
        _knownColors.Value.TryGetValue(colorParts[1], out falseColor);

      return isTrue
        ? new SolidColorBrush(trueColor)
        : new SolidColorBrush(falseColor);

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
