using System;
using System.Windows;
using System.Windows.Data;

namespace LoreSoft.Shared.Controls
{
  /// <summary>
  /// A framework element that permits a binding to be evaluated in a new data context leaf node.
  /// </summary>
  /// <typeparam name="T">The type of dynamic binding to return.</typeparam>
  public class BindingEvaluator<T> : FrameworkElement
  {
    #region Value
    /// <summary>
    /// Gets the data item value.
    /// </summary>
    private T Value
    {
      get { return (T)GetValue(ValueProperty); }
    }

    /// <summary>
    /// Identifies the Value dependency property.
    /// </summary>
    private static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            "Value",
            typeof(T),
            typeof(BindingEvaluator<T>),
            new PropertyMetadata(default(T)));
    #endregion Value

    /// <summary>
    /// Gets the value binding that is used as a template for the actual evaluation.
    /// </summary>
    public Binding ValueBinding { get; private set; }

    /// <summary>
    /// Initializes a new instance of the BindingEvaluator class.
    /// </summary>
    /// <param name="binding">The binding.</param>
    public BindingEvaluator(Binding binding)
    {
      ValueBinding = binding;
    }

    /// <summary>
    /// Evaluates the specified source.
    /// </summary>
    /// <param name="source">The object used as a source for the evaluation.</param>
    /// <returns>The evaluated binding.</returns>
    public T GetBindingValue(object source)
    {
      // a binding cannot be altered after it has been used.
      var copy = new Binding()
      {
        BindsDirectlyToSource = ValueBinding.BindsDirectlyToSource,
        Converter = ValueBinding.Converter,
        ConverterCulture = ValueBinding.ConverterCulture,
        ConverterParameter = ValueBinding.ConverterParameter,
        FallbackValue = ValueBinding.FallbackValue,
        Mode = ValueBinding.Mode,
        NotifyOnValidationError = ValueBinding.NotifyOnValidationError,
        Path = ValueBinding.Path,
        StringFormat = ValueBinding.StringFormat,
        TargetNullValue = ValueBinding.TargetNullValue,
        UpdateSourceTrigger = ValueBinding.UpdateSourceTrigger,
        ValidatesOnDataErrors = ValueBinding.ValidatesOnDataErrors,
        ValidatesOnExceptions = ValueBinding.ValidatesOnExceptions,
#if SILVERLIGHT
        ValidatesOnNotifyDataErrors = ValueBinding.ValidatesOnNotifyDataErrors,
#endif
      };
      copy.Source = source;

      SetBinding(ValueProperty, copy);
      T evaluatedValue = Value;
      ClearValue(ValueProperty);
      return evaluatedValue;
    }

    /// <summary>
    /// Evaluates the specified source.
    /// </summary>
    /// <param name="source">The object used as a source for the evaluation.</param>
    /// <param name="binding">The binding.</param>
    /// <returns>The evaluated binding.</returns>
    public static T GetBindingValue(Binding binding, object source)
    {
      var eval = new BindingEvaluator<T>(binding);
      return eval.GetBindingValue(source);
    }

    /// <summary>
    /// Evaluates the specified source.
    /// </summary>
    /// <param name="bindingExpression">The binding.</param>
    /// <returns>The evaluated binding.</returns>
    public static T GetBindingValue(BindingExpression bindingExpression)
    {
      var eval = new BindingEvaluator<T>(bindingExpression.ParentBinding);
      return eval.GetBindingValue(bindingExpression.DataItem);
    }
  }
}
