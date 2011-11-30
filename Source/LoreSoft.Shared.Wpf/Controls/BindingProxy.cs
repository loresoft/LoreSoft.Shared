using System;
using System.ComponentModel;
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
  public class BindingProxy
  {
    public IValueConverter Converter { get; set; }
    public CultureInfo ConverterCulture { get; set; }
    public object ConverterParameter { get; set; }
    public string Path { get; set; }
    public object Source { get; set; }
    public object FallbackValue { get; set; }
    public string StringFormat { get; set; }
    public object TargetNullValue { get; set; }

    public object Value { get; set; }

    public override string ToString()
    {
      var binding = new Binding(Path)
      {
        Converter = Converter,
        ConverterCulture = ConverterCulture,
        ConverterParameter = ConverterParameter,
        Mode = BindingMode.OneTime,
        FallbackValue = FallbackValue,
        StringFormat = StringFormat,
        TargetNullValue = TargetNullValue
      };

      Value = BindingEvaluator<object>.GetBindingValue(binding, Source);

      return Value.ToString();
    }
  }
}
