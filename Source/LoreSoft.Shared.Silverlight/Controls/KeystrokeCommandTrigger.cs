using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Expression.Interactivity.Input;

namespace LoreSoft.Shared.Controls
{
  public class KeystrokeCommandTrigger : KeyTrigger
  {
    #region Command
    public static readonly DependencyProperty CommandProperty =
      DependencyProperty.RegisterAttached(
        "Command",
        typeof(ICommand),
        typeof(KeystrokeCommandTrigger),
        new PropertyMetadata(OnCommandChanged));

    public static ICommand GetCommand(DependencyObject d)
    {
      return (ICommand)d.GetValue(CommandProperty);
    }
    public static void SetCommand(DependencyObject d, ICommand command)
    {
      d.SetValue(CommandProperty, command);
    }

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var actions = d.GetValue(ActionsProperty) as System.Windows.Interactivity.TriggerActionCollection;
      if (actions == null)
        return;

      var commandActions = actions.OfType<InvokeCommandAction>().ToList();

      var oldCommand = e.OldValue as ICommand;
      var oldCommandAction = commandActions.FirstOrDefault(a => a.Command == oldCommand);
      if (oldCommandAction != null)
        actions.Remove(oldCommandAction);

      var newCommand = e.NewValue as ICommand;
      var newCommandAction = commandActions.FirstOrDefault(a => a.Command == newCommand); ;
      if (newCommandAction != null)
        return;

      actions.Add(new InvokeCommandAction
      {
        Command = GetCommand(d),
        CommandParameter = GetCommandParameter(d)
      });
    }
    #endregion

    #region CommandParameter
    public static readonly DependencyProperty CommandParameterProperty =
      DependencyProperty.RegisterAttached(
        "CommandParameter",
        typeof(object),
        typeof(KeystrokeCommandTrigger),
        null);

    public static object GetCommandParameter(DependencyObject d)
    {
      return d.GetValue(CommandParameterProperty);
    }
    public static void SetCommandParameter(DependencyObject d, object o)
    {
      d.SetValue(CommandParameterProperty, o);
    }
    #endregion

    #region Gesture
    public static readonly DependencyProperty GestureProperty =
      DependencyProperty.RegisterAttached(
        "Gesture",
        typeof(string),
        typeof(KeystrokeCommandTrigger),
        new PropertyMetadata(OnGestureChanged));

    public static string GetGesture(DependencyObject d)
    {
      return (string)d.GetValue(GestureProperty);
    }
    public static void SetGesture(DependencyObject d, string gesture)
    {
      d.SetValue(GestureProperty, gesture);
    }

    private static void OnGestureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      string gesture = (string)e.NewValue;
      ModifierKeys modifierKeys = ModifierKeys.None;
      Key key = Key.None;

      ParseGesture(gesture, out modifierKeys, out key);

      d.SetValue(ModifiersProperty, modifierKeys);
      d.SetValue(KeyProperty, key);
    }
    #endregion

    private static readonly Regex _gestureRx = new Regex(@"^\w+(\s*\+\s*\w+)*$", RegexOptions.IgnoreCase);
    private static readonly Dictionary<string, ModifierKeys> _modifierMappings = new Dictionary<string, ModifierKeys>()
	  {
		  { "ctrl", ModifierKeys.Control }
	  };
    private static readonly Dictionary<string, Key> _keyMappings = new Dictionary<string, Key>()
	  {
		  { "del", Key.Delete },
		  { "ins", Key.Insert },
		  { "esc", Key.Escape }
	  };

    /// <summary>
    /// Parses string gesture into the set of modifier keys and the main key.
    /// </summary>
    /// <param name="gesture">String gesture representation.</param>
    /// <param name="modifierKeys">Modifier keys retuen value.</param>
    /// <param name="key">Key return value.</param>
    public static void ParseGesture(string gesture, out ModifierKeys modifierKeys, out Key key)
    {
      if (string.IsNullOrEmpty(gesture))
        throw new ArgumentException("Gesture can not be empty string or null");

      if (!_gestureRx.IsMatch(gesture))
        throw new ArgumentException("Wrong gesture format, should be something like 'Ctrl+E' or 'Ctrl+Shift+D'", "gesture");

      modifierKeys = ModifierKeys.None;
      key = Key.None;
      var tokens = gesture.Split('+');

      for (int i = 0; i < tokens.Length; i++)
      {
        var token = tokens[i].Trim().ToLower();

        // We have modifier keys at the beginning.
        if (i < tokens.Length - 1)
        {
          try
          {
            ModifierKeys modifierKey = _modifierMappings.ContainsKey(token)
              ? _modifierMappings[token]
              : (ModifierKeys)Enum.Parse(typeof(ModifierKeys), token, true);

            modifierKeys |= modifierKey;
          }
          catch (ArgumentException ex)
          {
            throw new FormatException(string.Format("Could not recognize {0} key", token), ex);
          }
        }
        // This is the main key.
        else
        {
          try
          {
            key = _keyMappings.ContainsKey(token)
              ? _keyMappings[token]
              : (Key)Enum.Parse(typeof(Key), token, true);
          }
          catch (ArgumentException ex)
          {
            throw new FormatException(string.Format("Could not recognize {0} key", token), ex);
          }
        }
      }
    }    
  }
}
