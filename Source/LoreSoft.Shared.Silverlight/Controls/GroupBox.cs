using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LoreSoft.Shared.Controls
{
  [TemplatePart(Name = ElementBodyRectangletName, Type = typeof(RectangleGeometry))]
  [TemplatePart(Name = ElementHeaderRectangleName, Type = typeof(RectangleGeometry))]
  [TemplatePart(Name = ElementHeaderContainerName, Type = typeof(ContentControl))]
  public class GroupBox : ContentControl
  {
    private const string ElementBodyRectangletName = "BodyRectangle";
    private const string ElementHeaderRectangleName = "HeaderRectangle";
    private const string ElementHeaderContainerName = "HeaderContainer";

    private RectangleGeometry BodyRectangle;
    private RectangleGeometry HeaderRectangle;
    private ContentControl HeaderContainer;

    public GroupBox()
    {
      DefaultStyleKey = typeof(GroupBox);
      this.SizeChanged += GroupBox_SizeChanged;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      BodyRectangle = (RectangleGeometry)GetTemplateChild(ElementBodyRectangletName);
      HeaderRectangle = (RectangleGeometry)GetTemplateChild(ElementHeaderRectangleName);
      HeaderContainer = (ContentControl)GetTemplateChild(ElementHeaderContainerName);
      HeaderContainer.SizeChanged += HeaderContainer_SizeChanged;
    }

    #region Header
    public object Header
    {
      get { return GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
      DependencyProperty.Register(
        "Header",
        typeof(object),
        typeof(GroupBox),
        null);
    #endregion

    #region HeaderTemplate
    public DataTemplate HeaderTemplate
    {
      get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
      set { SetValue(HeaderTemplateProperty, value); }
    }

    public static readonly DependencyProperty HeaderTemplateProperty =
      DependencyProperty.Register(
        "HeaderTemplate",
        typeof(DataTemplate),
        typeof(GroupBox),
        null);
    #endregion

    private void GroupBox_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
      BodyRectangle.Rect = new Rect(new Point(), e.NewSize);
    }

    private void HeaderContainer_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
      HeaderRectangle.Rect = new Rect(new Point(HeaderContainer.Margin.Left, 0), e.NewSize);
    }
  }
}
