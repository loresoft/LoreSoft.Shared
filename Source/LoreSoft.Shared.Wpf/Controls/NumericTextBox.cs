using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Controls
{
  public class NumericTextBox : TextBox
  {
    private const int UNDO_SIZE = 30;
    private const string DEFAULT_VALUE = "0";
    // no undo support for textbox, create our own.  
    // not using Stack<T> because its inefficient at limiting the size.
    private string[] _undoStack = new string[UNDO_SIZE];
    private int _undoIndex = 0;
    private bool _hasFocus = false;

    public NumericTextBox()
    {
      DefaultStyleKey = typeof(TextBox);

      Text = DEFAULT_VALUE;
      TextChanged += OnTextChanged;
    }

    #region AllowLeadingWhite
    public bool AllowLeadingWhite
    {
      get { return (bool)GetValue(AllowLeadingWhiteProperty); }
      set { SetValue(AllowLeadingWhiteProperty, value); }
    }

    public static readonly DependencyProperty AllowLeadingWhiteProperty =
        DependencyProperty.Register(
            "AllowLeadingWhite",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(false));
    #endregion

    #region AllowTrailingWhite
    public bool AllowTrailingWhite
    {
      get { return (bool)GetValue(AllowTrailingWhiteProperty); }
      set { SetValue(AllowTrailingWhiteProperty, value); }
    }

    public static readonly DependencyProperty AllowTrailingWhiteProperty =
        DependencyProperty.Register(
            "AllowTrailingWhite",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(false));
    #endregion

    #region AllowLeadingSign
    public bool AllowLeadingSign
    {
      get { return (bool)GetValue(AllowLeadingSignProperty); }
      set { SetValue(AllowLeadingSignProperty, value); }
    }

    public static readonly DependencyProperty AllowLeadingSignProperty =
        DependencyProperty.Register(
            "AllowLeadingSign",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(true));
    #endregion

    #region AllowTrailingSign
    public bool AllowTrailingSign
    {
      get { return (bool)GetValue(AllowTrailingSignProperty); }
      set { SetValue(AllowTrailingSignProperty, value); }
    }

    public static readonly DependencyProperty AllowTrailingSignProperty =
        DependencyProperty.Register(
            "AllowTrailingSign",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(false));
    #endregion

    #region AllowParentheses
    public bool AllowParentheses
    {
      get { return (bool)GetValue(AllowParenthesesProperty); }
      set { SetValue(AllowParenthesesProperty, value); }
    }

    public static readonly DependencyProperty AllowParenthesesProperty =
        DependencyProperty.Register(
            "AllowParentheses",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(false));
    #endregion

    #region AllowDecimalPoint
    public bool AllowDecimalPoint
    {
      get { return (bool)GetValue(AllowDecimalPointProperty); }
      set { SetValue(AllowDecimalPointProperty, value); }
    }

    public static readonly DependencyProperty AllowDecimalPointProperty =
        DependencyProperty.Register(
            "AllowDecimalPoint",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(true));
    #endregion

    #region AllowThousands
    public bool AllowThousands
    {
      get { return (bool)GetValue(AllowThousandsProperty); }
      set { SetValue(AllowThousandsProperty, value); }
    }

    public static readonly DependencyProperty AllowThousandsProperty =
        DependencyProperty.Register(
            "AllowThousands",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(false));
    #endregion

    #region AllowCurrencySymbol
    public bool AllowCurrencySymbol
    {
      get { return (bool)GetValue(AllowCurrencySymbolProperty); }
      set { SetValue(AllowCurrencySymbolProperty, value); }
    }

    public static readonly DependencyProperty AllowCurrencySymbolProperty =
        DependencyProperty.Register(
            "AllowCurrencySymbol",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(false));
    #endregion

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
      // handling text changed for things like cut, paste and setting Text directly.

      // when text is valid, add to undo stack
      double number;
      if (TryParse(Text, out number))
      {
        UndoPush(Text);
        return;
      }

      // allow empty string when control has focus
      if (_hasFocus && Text == string.Empty)
        return;

      // when text is invalid, revert to previous text 

      // remember selection
      int start = SelectionStart;
      int length = SelectionLength;

      Text = UndoPop() ?? string.Empty;

      // restore
      Select(start, length);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      _hasFocus = true; 

      double number;
      TryParse(Text, out number);
      if (number == 0)
        SelectAll();

      base.OnGotFocus(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {      
      _hasFocus = false;

      if (Text == string.Empty)
        Text = DEFAULT_VALUE;

      base.OnLostFocus(e);
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
      // works by creating the resulting string, then trying to parse it.
      // if resulting string can be parsed to a double, allow TextInput
      // by not handling it here.

      string result = PreProcess(e.Text);
      double number;
      bool isNumber = TryParse(result, out number);

      // prevent textbox from handling new text when result is not a number
      e.Handled = !isNumber;

      base.OnTextInput(e);
    }

    private string UndoPop()
    {
      if (_undoIndex == 0)
      {
        // try getting binding value
        var binding = GetBindingExpression(TextProperty);
        if (binding != null)
          return BindingEvaluator<string>.GetBindingValue(binding);

        return DEFAULT_VALUE;
      }

      string text = _undoStack[--_undoIndex];
      _undoStack[_undoIndex] = null;
      return text;
    }

    private void UndoPush(string text)
    {
      if (_undoIndex == _undoStack.Length)
      {
        // remove half to prevent excessive array changes
        int remove = _undoIndex / 2;
        _undoIndex = _undoIndex - remove;

        //copy last items to front of new array
        var newArray = new string[UNDO_SIZE];
        Array.Copy(_undoStack, remove, newArray, 0, _undoIndex);

        _undoStack = newArray;
      }

      _undoStack[_undoIndex++] = text;
    }

    private bool TryParse(string text, out double number)
    {
      var culture = CultureInfo.CurrentCulture;
      NumberStyles styles = GetAllowedStyles();
      
      if (text.StartsWith(culture.NumberFormat.NumberDecimalSeparator))
        text = "0" + text;

      return double.TryParse(text, styles, culture, out number);
    }

    private string PreProcess(string input)
    {
      string original = Text;
      int start = SelectionStart;
      int length = SelectionLength;

      string head = original.Substring(0, start);
      string tail = original.Substring(start + length);

      return head + input + tail;
    }

    private NumberStyles GetAllowedStyles()
    {
      NumberStyles current = NumberStyles.None;

      if (AllowLeadingWhite)
        current = current.SetFlagOn(NumberStyles.AllowLeadingWhite);

      if (AllowTrailingWhite)
        current = current.SetFlagOn(NumberStyles.AllowTrailingWhite);

      if (AllowLeadingSign)
        current = current.SetFlagOn(NumberStyles.AllowLeadingSign);

      if (AllowTrailingSign)
        current = current.SetFlagOn(NumberStyles.AllowTrailingSign);

      if (AllowParentheses)
        current = current.SetFlagOn(NumberStyles.AllowParentheses);

      if (AllowDecimalPoint)
        current = current.SetFlagOn(NumberStyles.AllowDecimalPoint);

      if (AllowThousands)
        current = current.SetFlagOn(NumberStyles.AllowThousands);

      if (AllowCurrencySymbol)
        current = current.SetFlagOn(NumberStyles.AllowCurrencySymbol);

      return current;
    }
  }
}
