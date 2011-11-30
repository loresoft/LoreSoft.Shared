using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LoreSoft.Shared.Controls
{
  public class NumericConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return System.Convert.ChangeType(value, targetType, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      // safely convert to various numeric types
      if (targetType == typeof(decimal))
      {
        if (value == null)
          return 0;

        return decimal.Parse(value.ToString(), culture);
      }

      if (targetType == typeof(float))
      {
        if (value == null)
          return 0;

        return float.Parse(value.ToString(), culture);
      }
      
      if (targetType == typeof(double))
      {
        if (value == null)
          return 0;

        return double.Parse(value.ToString(), culture);
      }
      
      if (targetType == typeof(sbyte))
      {
        if (value == null)
          return 0;

        return sbyte.Parse(value.ToString(), culture);
      }
      
      if (targetType == typeof(byte))
      {
        if (value == null)
          return 0;

        return byte.Parse(value.ToString(), culture);
      }
      
      if (targetType == typeof(short))
      {
        if (value == null)
          return 0;

        return short.Parse(value.ToString(), culture);
      }
      
      if (targetType == typeof(ushort))
      {
        if (value == null)
          return 0;

        return ushort.Parse(value.ToString(), culture);
      }
      
      if (targetType == typeof(int))
      {
        if (value == null)
          return 0;

        return int.Parse(value.ToString(), culture);
      }
      
      if (targetType == typeof(uint))
      {
        if (value == null)
          return 0;

        return uint.Parse(value.ToString(), culture);
      }
      
      if (targetType == typeof(long))
      {
        if (value == null)
          return 0;

        return long.Parse(value.ToString(), culture);
      }
      
      if (targetType == typeof(ulong))
      {
        if (value == null)
          return 0;

        return long.Parse(value.ToString(), culture);
      }

      return System.Convert.ChangeType(value, targetType, culture);
    }
  }
}
