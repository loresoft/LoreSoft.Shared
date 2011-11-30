using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using LoreSoft.Shared.Collections;
using LoreSoft.Shared.Command;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Controls
{
  [TemplatePart(Name = ElementCloseButtonName, Type = typeof(ButtonBase))]
  [TemplatePart(Name = ElementNextButtonName, Type = typeof(ButtonBase))]
  [TemplatePart(Name = ElementPreviousButtonName, Type = typeof(ButtonBase))]
  [TemplatePart(Name = ElementTitleTextBlockName, Type = typeof(TextBlock))]
  [TemplatePart(Name = ElementMessageTextBlockName, Type = typeof(TextBlock))]
  [TemplatePart(Name = ElementDetailTextBlockName, Type = typeof(TextBlock))]
  [TemplatePart(Name = ElementDetailPopupName, Type = typeof(Popup))]
  public class Notifications : ItemsControl
  {
    private const string ElementTitleTextBlockName = "TitleTextBlock";
    private const string ElementMessageTextBlockName = "MessageTextBlock";
    private const string ElementDetailTextBlockName = "DetailTextBlock";
    private const string ElementCloseButtonName = "CloseButton";
    private const string ElementNextButtonName = "NextButton";
    private const string ElementPreviousButtonName = "PreviousButton";
    private const string ElementDetailPopupName = "DetailPopup";
    private const string ElementCountBorderName = "CountBorder";
    private const string ElementCountTextBlockName = "CountTextBlock";
    
    private ButtonBase ElementCloseButton;
    private ButtonBase ElementNextButton;
    private ButtonBase ElementPreviousButton;
    private TextBlock ElementTitleTextBlock;
    private TextBlock ElementMessageTextBlock;
    private TextBlock ElementDetailTextBlock;
    private Popup ElementDetailPopup;
    private FrameworkElement ElementDetailPopupChild;
    private Border ElementCountBorder;
    private TextBlock ElementCountTextBlock;

    private bool _isMouseOver;

    public Notifications()
    {
      DefaultStyleKey = typeof(Notifications);

      CloseCommand = new RelayCommand<object>(CloseItem);
      NextCommand = new RelayCommand(MoveNext, CanMoveNext);
      PreviousCommand = new RelayCommand(MovePrevious, CanMovePrevious);
    }

    
    private void ArrangePopup()
    {
      if (ElementDetailPopup == null || ElementDetailPopupChild == null || ElementDetailTextBlock == null)
        return;

      ElementDetailPopup.Width = ActualWidth;
      ElementDetailPopupChild.Width = ActualWidth;
      ElementDetailTextBlock.Width = ActualWidth - 40;
    }

    private void ElementPopupChild_MouseLeave(object sender, MouseEventArgs e)
    {
      _isMouseOver = false;
      HideDetail();
    }

    private void ElementPopupChild_MouseEnter(object sender, MouseEventArgs e)
    {
      _isMouseOver = true;
      ShowDetail();
    }

    private void ElementPopupChild_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ArrangePopup();
    }

    
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      ElementCloseButton = GetTemplateChild(ElementCloseButtonName) as ButtonBase;
      ElementNextButton = GetTemplateChild(ElementNextButtonName) as ButtonBase;
      ElementPreviousButton = GetTemplateChild(ElementPreviousButtonName) as ButtonBase;

      ElementTitleTextBlock = GetTemplateChild(ElementTitleTextBlockName) as TextBlock;
      ElementMessageTextBlock = GetTemplateChild(ElementMessageTextBlockName) as TextBlock;

      ElementDetailTextBlock = GetTemplateChild(ElementDetailTextBlockName) as TextBlock;
      ElementDetailPopup = GetTemplateChild(ElementDetailPopupName) as Popup;

      ElementDetailPopupChild = ElementDetailPopup != null
        ? ElementDetailPopup.Child as FrameworkElement
        : null;

      SizeChanged += ElementPopupChild_SizeChanged;
      if (ElementDetailPopupChild != null)
      {
        ElementDetailPopupChild.MouseEnter += ElementPopupChild_MouseEnter;
        ElementDetailPopupChild.MouseLeave += ElementPopupChild_MouseLeave;
        ElementDetailPopupChild.SizeChanged += ElementPopupChild_SizeChanged;
      }

      ElementCountBorder = GetTemplateChild(ElementCountBorderName) as Border;
      ElementCountTextBlock = GetTemplateChild(ElementCountTextBlockName) as TextBlock;

      UpdateBinding();
      UpdateCommands();
      UpdateVisibility();
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);

      int index = e.Action == NotifyCollectionChangedAction.Remove
        ? e.OldStartingIndex
        : e.NewStartingIndex;

      index = CoerceIndex(index);

      // when collection changes, need to rebind even if index is same
      if (CurrentIndex == index)
        UpdateBinding();
      else
        CurrentIndex = index;

      UpdateCommands();
      UpdateVisibility();
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);
      _isMouseOver = true;
      ShowDetail();
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      _isMouseOver = false;
      HideDetail();
    }


    #region TitlePath
    public string TitlePath
    {
      get { return (string)GetValue(TitlePathProperty); }
      set { SetValue(TitlePathProperty, value); }
    }

    public static readonly DependencyProperty TitlePathProperty =
        DependencyProperty.Register(
          "TitlePath",
          typeof(string),
          typeof(Notifications),
          new PropertyMetadata("Title", OnTitlePathChanged));

    private static void OnTitlePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = (Notifications)d;
      source.OnTitlePathChanged(e);
    }

    protected virtual void OnTitlePathChanged(DependencyPropertyChangedEventArgs e)
    {
      // rebind
      UpdateBinding();
    }
    #endregion

    #region MessagePath
    public string MessagePath
    {
      get { return (string)GetValue(MessagePathProperty); }
      set { SetValue(MessagePathProperty, value); }
    }

    public static readonly DependencyProperty MessagePathProperty =
        DependencyProperty.Register(
          "MessagePath",
          typeof(string),
          typeof(Notifications),
          new PropertyMetadata("Message", OnMessagePathChanged));

    private static void OnMessagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = (Notifications)d;
      source.OnMessagePathChanged(e);
    }

    protected virtual void OnMessagePathChanged(DependencyPropertyChangedEventArgs e)
    {
      // rebind
      UpdateBinding();

    }
    #endregion

    #region DetailPath
    public string DetailPath
    {
      get { return (string)GetValue(DetailPathProperty); }
      set { SetValue(DetailPathProperty, value); }
    }

    public static readonly DependencyProperty DetailPathProperty =
        DependencyProperty.Register(
          "DetailPath",
          typeof(string),
          typeof(Notifications),
          new PropertyMetadata("Detail", OnDetailPathChanged));

    private static void OnDetailPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = (Notifications)d;
      source.OnDetailPathChanged(e);
    }

    protected virtual void OnDetailPathChanged(DependencyPropertyChangedEventArgs e)
    {
      // rebind
      UpdateBinding();
    }
    #endregion


    #region CloseCommand
    /// <summary>
    /// Gets or sets the command associated with the close notification action.
    /// </summary>
    public ICommand CloseCommand
    {
      get { return (ICommand)GetValue(CloseCommandProperty); }
      set { SetValue(CloseCommandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CloseCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(
        "CloseCommand",
        typeof(ICommand),
        typeof(Notifications),
        new PropertyMetadata(null));
    #endregion CloseCommand

    #region NextCommand
    /// <summary>
    /// Gets or sets the command associated with the move next notification action.
    /// </summary>
    public ICommand NextCommand
    {
      get { return (ICommand)GetValue(NextCommandProperty); }
      set { SetValue(NextCommandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="NextCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NextCommandProperty = DependencyProperty.Register(
        "NextCommand",
        typeof(ICommand),
        typeof(Notifications),
        new PropertyMetadata(null));
    #endregion NextCommand

    #region PreviousCommand
    /// <summary>
    /// Gets or sets the command associated with the move previous notification action.
    /// </summary>
    public ICommand PreviousCommand
    {
      get { return (ICommand)GetValue(PreviousCommandProperty); }
      set { SetValue(PreviousCommandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PreviousCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PreviousCommandProperty = DependencyProperty.Register(
        "PreviousCommand",
        typeof(ICommand),
        typeof(Notifications),
        new PropertyMetadata(null));
    #endregion PreviousCommand


    #region IsCyclic
    /// <summary>
    /// Gets or sets a value indicating whether the DomainUpDown control 
    /// will cycle through values when trying to spin the first and last item. 
    /// </summary>
    public bool IsCyclic
    {
      get { return (bool)GetValue(IsCyclicProperty); }
      set { SetValue(IsCyclicProperty, value); }
    }

    /// <summary>
    /// Identifies the IsCyclic dependency property.
    /// </summary>
    public static readonly DependencyProperty IsCyclicProperty =
        DependencyProperty.Register(
            "IsCyclic",
            typeof(bool),
            typeof(Notifications),
            new PropertyMetadata(false));
    #endregion IsCyclic

    #region CurrentIndex
    private int _currentIndexLevel;

    public int CurrentIndex
    {
      get { return (int)GetValue(CurrentIndexProperty); }
      set { SetValue(CurrentIndexProperty, value); }
    }

    public static readonly DependencyProperty CurrentIndexProperty =
        DependencyProperty.Register(
        "CurrentIndex",
        typeof(int),
        typeof(Notifications),
        new PropertyMetadata(-1, OnCurrentIndexChanged));

    private static void OnCurrentIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = (Notifications)d;
      int newValue = (int)e.NewValue;
      int oldValue = (int)e.OldValue;

      source._currentIndexLevel++;

      // coerce newValue
      int coercedValue = source.CoerceIndex(newValue);
      // always set CurrentIndex to coerced value
      if (newValue != coercedValue)
        source.CurrentIndex = coercedValue;

      source._currentIndexLevel--;

      // fire changed event only at root level and when there is indeed a change
      if (source._currentIndexLevel == 0)
        source.OnCurrentIndexChanged(oldValue, source.CurrentIndex);
    }

    protected virtual void OnCurrentIndexChanged(int oldValue, int newValue)
    {
      UpdateBinding();
    }

    private bool IsValidCurrentIndex(int value)
    {
      // -1 is only legal when the items collection is empty
      int count = Items.Count;
      return (value == -1 && count == 0) || (value >= 0 && value < count);
    }

    private int CoerceIndex(int index)
    {
      // no coercion needed
      if (IsValidCurrentIndex(index))
        return index;

      // if we are at an empty collection, return -1
      if (Items.Count == 0)
        return -1;

      // select the first item in the collection.
      return 0;
    }

    #endregion

    #region CurrentItem
    private bool _changingCurrentItemReadOnly;
    private bool _restoringCurrentItemReadOnly;

    public object CurrentItem
    {
      get
      {
        return GetValue(CurrentItemProperty);
      }
      protected set
      {
        try
        {
          _changingCurrentItemReadOnly = true;
          SetValue(CurrentItemProperty, value);
        }
        finally
        {
          _changingCurrentItemReadOnly = false;
        }
      }
    }

    public static readonly DependencyProperty CurrentItemProperty =
        DependencyProperty.Register(
          "CurrentItem",
          typeof(object),
          typeof(Notifications),
          new PropertyMetadata(null, OnCurrentItemChanged));

    private static void OnCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var notifications = (Notifications)d;
      if (notifications._changingCurrentItemReadOnly
        || notifications._restoringCurrentItemReadOnly)
        return;

      try
      {
        notifications._restoringCurrentItemReadOnly = true;
        notifications.CurrentItem = e.OldValue;
      }
      finally
      {
        notifications._restoringCurrentItemReadOnly = false;
      }
      throw new InvalidOperationException("'CurrentItem' property is read-only and cannot be modified.");
    }
    #endregion

    private bool _hasDetail;
    public bool HasDetail
    {
      get { return _hasDetail; }
    }

    public virtual void CloseItem(object item)
    {
      Items.Remove(item);
    }


    public bool CanMoveNext()
    {
      if (IsCyclic)
        return true;

      return IsValidCurrentIndex(CurrentIndex + 1);
    }

    public virtual void MoveNext()
    {
      if (IsValidCurrentIndex(CurrentIndex + 1))
      {
        CurrentIndex += 1;
      }
      else if (IsCyclic)
      {
        CurrentIndex = 0;
      }
    }

    public bool CanMovePrevious()
    {
      if (IsCyclic)
        return true;

      return CurrentIndex > 0;
    }

    public virtual void MovePrevious()
    {
      if (CurrentIndex > 0)
      {
        CurrentIndex -= 1;
      }
      else if (IsCyclic)
      {
        CurrentIndex = Items.Count - 1;
      }
    }

    
    protected virtual void ShowDetail()
    {
      if (!HasDetail || ElementDetailPopup == null)
        return;

      ElementDetailPopup.IsOpen = true;
    }

    protected virtual void HideDetail()
    {
      if (ElementDetailPopup == null)
        return;

      ElementDetailPopup.IsOpen = false;
    }


    private void UpdateBinding()
    {
      // the CurrentIndex is coerced, so is always valid.
      var source = Items.OfType<object>().ElementAtOrDefault(CurrentIndex);
      // update CurrentItem, only used for data binding
      CurrentItem = source;

      if (ElementTitleTextBlock != null)
      {
        var b = new Binding(TitlePath) { Source = source };
        ElementTitleTextBlock.SetBinding(TextBlock.TextProperty, b);
      }

      if (ElementMessageTextBlock != null)
      {
        var b = new Binding(MessagePath) { Source = source };
        ElementMessageTextBlock.SetBinding(TextBlock.TextProperty, b);
      }

      if (ElementDetailTextBlock != null)
      {
        var b = new Binding(DetailPath) { Source = source };
        ElementDetailTextBlock.SetBinding(TextBlock.TextProperty, b);

        // check if current item has detail value
        var e = new BindingEvaluator<string>(b);
        string d = e.GetBindingValue(source);
        _hasDetail = !string.IsNullOrWhiteSpace(d);
      }
      else
      {
        _hasDetail = false;
      }

      if (_isMouseOver)
      {
        if (_hasDetail)
          ShowDetail();
        else
          HideDetail();
      }
    }

    private void UpdateCommands()
    {
      if (NextCommand is RelayCommand)
        ((RelayCommand)NextCommand).RaiseCanExecuteChanged();
      if (PreviousCommand is RelayCommand)
        ((RelayCommand)NextCommand).RaiseCanExecuteChanged();
      if (CloseCommand is RelayCommand)
        ((RelayCommand)NextCommand).RaiseCanExecuteChanged();
    }

    private void UpdateVisibility()
    {
      int count = Items.Count;

      if (count == 0)
      {
        HideDetail();
        Visibility = Visibility.Collapsed;
      }
      else if (count == 1)
      {
        Visibility = Visibility.Visible;
        if (ElementNextButton != null)
          ElementNextButton.Visibility = Visibility.Collapsed;
        if (ElementPreviousButton != null)
          ElementPreviousButton.Visibility = Visibility.Collapsed;
        if (ElementCountBorder != null)
          ElementCountBorder.Visibility = Visibility.Collapsed;
      }
      else
      {
        Visibility = Visibility.Visible;
        if (ElementNextButton != null)
          ElementNextButton.Visibility = Visibility.Visible;
        if (ElementPreviousButton != null)
          ElementPreviousButton.Visibility = Visibility.Visible;

        if (ElementCountBorder != null)
        {
          ElementCountBorder.Visibility = Visibility.Visible;
          
          if (count.Between(1, 9))
            ElementCountBorder.Width = 20;
          else if (count.Between(10, 99))
            ElementCountBorder.Width = 30;
          else if (count.Between(100, 999))
            ElementCountBorder.Width = 40;
          else
            ElementCountBorder.Width = 50;
        }
        if (ElementCountTextBlock != null)
          ElementCountTextBlock.Text = count.ToString();
      }
    }
  }
}