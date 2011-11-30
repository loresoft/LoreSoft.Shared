using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace LoreSoft.Shared.Controls
{
  public class FilterBehavior : Behavior<AutoCompleteBox>
  {

    #region FilterCommand
    public ICommand FilterCommand
    {
      get { return (ICommand)GetValue(FilterCommandProperty); }
      set { SetValue(FilterCommandProperty, value); }
    }

    public static readonly DependencyProperty FilterCommandProperty =
        DependencyProperty.Register(
            "FilterCommand",
            typeof(ICommand),
            typeof(FilterBehavior),
            new PropertyMetadata(null, OnFilterCommandChanged));

    private static void OnFilterCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as FilterBehavior;
      if (source == null)
        return;

      source.OnFilterCommandChanged(e);
    }

    protected virtual void OnFilterCommandChanged(DependencyPropertyChangedEventArgs e)
    {

    }
    #endregion

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.Populating += OnPopulating;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.Populating -= OnPopulating;
      base.OnDetaching();
    }

    private void OnPopulating(object sender, PopulatingEventArgs e)
    {
      var command = FilterCommand;
      if (command == null)
        return;

      var parameter = new FilterParameter(AssociatedObject.PopulateComplete, e.Parameter);
      command.Execute(parameter);
      e.Cancel = true;
    }
  }

  public class FilterParameter
  {
    public FilterParameter(Action complete, string criteria)
    {
      Complete = complete;
      Criteria = criteria;
    }

    public Action Complete { get; private set; }

    public string Criteria { get; private set; }
  }

}
