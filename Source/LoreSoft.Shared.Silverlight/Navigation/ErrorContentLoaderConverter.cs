using System;
using System.ComponentModel;
using System.Globalization;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///   TypeConverter that converts a string into an INavigationContentLoader for the purposes of the ErrorPageLoader.
  /// </summary>
  public class ErrorContentLoaderConverter : TypeConverter
  {
    /// <summary>
    ///   Checks to see whether conversion is possible.
    /// </summary>
    /// <param name = "context">Context for the conversion.</param>
    /// <param name = "sourceType">The type from which conversion is being requested.</param>
    /// <returns>true if the sourceType is a string.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType.Equals(typeof (string));
    }

    /// <summary>
    ///   Converts a value to an INavigationContentLoader for the purposes of the ErrorPageLoader.
    /// </summary>
    /// <param name = "context">Context for the conversion.</param>
    /// <param name = "culture">Culture for the conversion.</param>
    /// <param name = "value">Value being converted.</param>
    /// <returns>ErrorRedirector if the string is "Redirect", <paramref name = "value" /> otherwise.</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string val = value.ToString();
      switch (val)
      {
        case "Redirect":
          return new ErrorRedirector();
      }
      return value;
    }
  }
}