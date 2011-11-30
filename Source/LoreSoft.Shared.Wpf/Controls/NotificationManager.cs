using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using LoreSoft.Shared.Command;
using LoreSoft.Shared.ComponentModel;
using LoreSoft.Shared.Extensions;
using LoreSoft.Shared.Messaging;

namespace LoreSoft.Shared.Controls
{
  public class NotificationManager
    : SingletonBase<NotificationManager>
  {
    private readonly ObservableCollection<Notification> _notifications;
    private readonly MessageSubscriber _subscriptions;
    private readonly RelayCommand _closeCommand;

    private readonly Popup _notificationPopup;
    private readonly Notifications _notificationControl;

    private UIElement _anchorElement;
    private bool _haveSearched;

    public NotificationManager()
    {
      _notifications = new ObservableCollection<Notification>();
      _subscriptions = new MessageSubscriber();
      _closeCommand = new RelayCommand<Notification>(Remove);
      _subscriptions.Subscribe<NotificationMessage>(OnNotificationMessage);

      _notificationControl = new Notifications();
      _notificationControl.CloseCommand = _closeCommand;
      _notificationControl.ItemsSource = _notifications;
      _notificationControl.SizeChanged += OnContentResized;

      _notificationPopup = new Popup();
      _notificationPopup.Child = _notificationControl;

      _notifications.CollectionChanged += OnNotificationsCollectionChanged;
    }

    private double _popupWidth;
    public double PopupWidth
    {
      get { return _popupWidth; }
      set
      {
        if (_popupWidth == value)
          return;

        _popupWidth = value;
        ArrangePopup();
      }
    }

    private string _popupAnchorElement;
    public string PopupAnchorElement
    {
      get { return _popupAnchorElement; }
      set
      {
        if (_popupAnchorElement == value)
          return;

        _popupAnchorElement = value;
        ArrangePopup();
      }
    }

    private void ArrangePopup()
    {
      if (Application.Current == null)
        return;

      UIElement anchorElement = FindAnchor();

      double width = PopupWidth;
      if (width == 0 && anchorElement != null)
        width = (double)anchorElement.GetValue(FrameworkElement.ActualWidthProperty);

      if (width == 0)
#if SILVERLIGHT
        width = Application.Current.Host.Content.ActualWidth;
#else
        width = Application.Current.MainWindow.ActualWidth;
#endif

      _notificationControl.Width = width;
      _notificationPopup.Width = width;

      if (anchorElement != null)
      {
#if SILVERLIGHT
        GeneralTransform gt = anchorElement.TransformToVisual(Application.Current.RootVisual);
#else
        GeneralTransform gt = anchorElement.TransformToVisual(Application.Current.MainWindow);
#endif
        Point offset = gt.Transform(new Point(0, 0));

        double horizontalOffset = Math.Max(0, offset.X);
        double verticalOffset = Math.Max(0, offset.Y - _notificationControl.ActualHeight);

        _notificationPopup.HorizontalOffset = horizontalOffset;
        _notificationPopup.VerticalOffset = verticalOffset;
      }
    }

    private UIElement FindAnchor()
    {
      if (string.IsNullOrEmpty(PopupAnchorElement))
        return null;

#if SILVERLIGHT
      if (Application.Current.RootVisual == null)
        return null;
#else
      if (Application.Current.MainWindow == null)
        return null;
#endif

      if (!_haveSearched)
      {
#if SILVERLIGHT
        _anchorElement = FindByName(Application.Current.RootVisual, PopupAnchorElement) as UIElement;
#else
       _anchorElement = FindByName(Application.Current.MainWindow, PopupAnchorElement) as UIElement;
#endif
        _haveSearched = true;
      }

      return _anchorElement;
    }

    private static DependencyObject FindByName(DependencyObject container, string name)
    {
      if (container == null || string.IsNullOrEmpty(name))
        return null;

      int childrenCount = VisualTreeHelper.GetChildrenCount(container);
      if (childrenCount == 0)
        return null;

      for (int i = 0; i < childrenCount; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(container, i);
        string value = child.GetValue(FrameworkElement.NameProperty) as string;

        if (string.Equals(value, name))
          return child;

        //recursively look at its children
        DependencyObject foundChild = FindByName(child, name);
        if (foundChild != null)
          return foundChild;
      }

      return null;
    }

    private void OnNotificationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      int count = _notifications.Count;
      _notificationPopup.IsOpen = count > 0;
    }

    private void OnContentResized(object sender, EventArgs e)
    {
      ArrangePopup();
    }

    public ObservableCollection<Notification> Notifications
    {
      get { return _notifications; }
    }

    public ICommand CloseCommand
    {
      get { return _closeCommand; }
    }

    public void Add(string title, string message)
    {
      Add(title, message, null);
    }

    public void Add(string title, string message, string group)
    {
      var notification = new Notification
      {
        Title = title,
        Message = message.RemoveInvisible(),
        Group = group
      };

      Add(notification);
    }

    public void Add(string title, Exception exception)
    {
      Add(title, exception, null);
    }

    public void Add(string title, Exception exception, string group)
    {
      var ex = exception;

      var notification = new Notification
      {
        Title = title,
        Message = ex.GetBaseException().Message.RemoveInvisible(),
        //Detail = ex.ToString(),
        Group = group
      };

      Add(notification);
    }

    public void Add(Notification notification)
    {
      _notifications.Add(notification);
    }

    public void Clear()
    {
      _notifications.Clear();
    }

    public void RemoveValidation()
    {
      _notifications.RemoveAll(n => n.IsValidation);
    }

    public void Remove(string group)
    {
      _notifications.RemoveAll(n => string.Equals(n.Group, group, StringComparison.OrdinalIgnoreCase));
    }

    public void Remove(Notification notification)
    {
      _notifications.Remove(notification);
    }

    public virtual void OnNotificationMessage(NotificationMessage message)
    {
      if (message == null || message.Notification == null)
        return;

      if (!message.Notification.Group.IsNullOrEmpty()
        && !string.Equals(GetType().Name, message.Notification.Group))
        return;

      if (message.ClearExisting)
        Notifications.Clear();

      Notifications.Add(message.Notification);
    }

  }
}
