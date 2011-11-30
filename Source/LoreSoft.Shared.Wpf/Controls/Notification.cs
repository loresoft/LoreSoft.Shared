using System;
using LoreSoft.Shared.ComponentModel;

namespace LoreSoft.Shared.Controls
{
  public class Notification : NotificationBase
  {
    public Notification()
    { }

    public Notification(string title, string message)
    {
      _title = title;
      _message = message;
    }

    public Notification(string title, Exception exception)
    {
      _title = title;
      _message = exception.GetBaseException().Message;
      _detail = exception.ToString();
    }

    private string _title;
    public string Title
    {
      get { return _title; }
      set
      {
        if (_title == value)
          return;

        _title = value;
        RaisePropertyChanged(() => Title);
      }
    }

    private string _message;
    public string Message
    {
      get { return _message; }
      set
      {
        if (_message == value)
          return;

        _message = value;
        RaisePropertyChanged(() => Message);
      }
    }

    private string _detail;
    public string Detail
    {
      get { return _detail; }
      set
      {
        if (_detail == value)
          return;

        _detail = value;
        RaisePropertyChanged(() => Detail);
      }
    }

    private object _tag;
    public object Tag
    {
      get { return _tag; }
      set
      {
        if (_tag == value)
          return;

        _tag = value;
        RaisePropertyChanged(() => Tag);
      }
    }

    private bool _isValidation;
    public bool IsValidation
    {
      get { return _isValidation; }
      set
      {
        if (_isValidation == value)
          return;

        _isValidation = value;
        RaisePropertyChanged(() => IsValidation);
      }
    }

    private string _group;
    public string Group
    {
      get { return _group; }
      set
      {
        if (_group == value)
          return;

        _group = value;
        RaisePropertyChanged(() => Group);
      }
    }

    public Notification WithGroup(string group)
    {
      Group = group;
      return this;
    }

    public Notification WithTitle(string title)
    {
      Title = title;
      return this;
    }

    public Notification WithMessage(string message)
    {
      Message = message;
      return this;
    }

    public Notification WithDetail(string detail)
    {
      Detail = detail;
      return this;
    }

    public Notification WithTag(object tag)
    {
      Tag = tag;
      return this;
    }

#if SILVERLIGHT
    public void Notify()
    {
      NotificationManager.Current.Add(this);
    }
#endif
  }
}