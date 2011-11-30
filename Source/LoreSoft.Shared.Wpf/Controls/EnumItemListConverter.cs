using System;
using System.Linq;
using System.Windows.Data;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Controls
{
  public class EnumItemListConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var type = value.GetType();      
      return EnumItem.CreateList(type).ToList();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}