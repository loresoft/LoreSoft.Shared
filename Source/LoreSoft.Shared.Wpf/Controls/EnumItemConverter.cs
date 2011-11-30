using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Data;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Controls
{
  public class EnumItemConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return targetType.Default();

      return EnumItem.Create(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return targetType.Default();

      var item = value as EnumItem;
      if (item != null)
      {
        if (item.OriginalValue != null && item.OriginalValue.GetType() == targetType)
          return item.OriginalValue;

        // if diff type, try matching name
        return Enum.Parse(targetType, item.Name, true);
      }
      
      if (value is string)
        return Enum.Parse(targetType, (string)value, true);

      return Enum.ToObject(targetType, value);
    }
  }
}
