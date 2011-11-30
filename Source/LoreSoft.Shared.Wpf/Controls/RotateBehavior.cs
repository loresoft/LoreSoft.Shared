using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace LoreSoft.Shared.Controls
{
  public class RotateBehavior : Behavior<Panel>
  {
    private bool _isRotating;
    private Point _mouseDownPoint;
    private double _actualAngle;

    #region RotateElement (DependencyProperty)
    /// <summary>
    /// Gets or sets the element name to rotate. The element must have its Transform set to <see cref="RotateTransform"/>.
    /// </summary>
    public string RotateElement
    {
      get { return (string)GetValue(RotateElementProperty); }
      set { SetValue(RotateElementProperty, value); }
    }

    public static readonly DependencyProperty RotateElementProperty =
        DependencyProperty.Register(
          "RotateElement", 
          typeof(string), 
          typeof(RotateBehavior),
          new PropertyMetadata(null));

    #endregion

    #region SnapPoints (DependencyProperty)
    /// <summary>
    /// Gets or sets the number snap points to snap the rotation to.
    /// </summary>
    public int SnapPoints
    {
      get { return (int)GetValue(SnapPointsProperty); }
      set { SetValue(SnapPointsProperty, value); }
    }

    public static readonly DependencyProperty SnapPointsProperty =
        DependencyProperty.Register(
          "SnapPoints", 
          typeof(int), 
          typeof(RotateBehavior),
          new PropertyMetadata(8));

    #endregion

    #region IsSnapEnabled (DependencyProperty)
    public bool IsSnapEnabled
    {
      get { return (bool)GetValue(IsSnapEnabledProperty); }
      set { SetValue(IsSnapEnabledProperty, value); }
    }
    public static readonly DependencyProperty IsSnapEnabledProperty =
        DependencyProperty.Register(
          "IsSnapEnabled", 
          typeof(bool), 
          typeof(RotateBehavior),
          new PropertyMetadata(true));

    #endregion

    #region IsRotateEnabled (DependencyProperty)

    /// <summary>
    /// A description of the property.
    /// </summary>
    public bool IsRotateEnabled
    {
      get { return (bool)GetValue(IsRotateEnabledProperty); }
      set { SetValue(IsRotateEnabledProperty, value); }
    }
    public static readonly DependencyProperty IsRotateEnabledProperty =
        DependencyProperty.Register(
          "IsRotateEnabled", 
          typeof(bool), 
          typeof(RotateBehavior),
          new PropertyMetadata(true));

    #endregion

    #region Angle (DependencyProperty)
    public double Angle
    {
      get { return (double)GetValue(AngleProperty); }
      set { SetValue(AngleProperty, value); }
    }

    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(
            "Angle",
            typeof(double),
            typeof(RotateBehavior),
            new PropertyMetadata(0D, OnAngleChanged));

    private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as RotateBehavior;
      if (source == null)
        return;

      source.OnAngleChanged(e);
    }

    protected virtual void OnAngleChanged(DependencyPropertyChangedEventArgs e)
    {

    }
    #endregion
    
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
      AssociatedObject.MouseMove += OnMouseMove;
      AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
      AssociatedObject.Cursor = Cursors.Hand;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
      AssociatedObject.MouseMove -= OnMouseMove;
      AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
      AssociatedObject.Cursor = Cursors.Arrow;
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (!IsRotateEnabled)
        return;

      _isRotating = AssociatedObject.CaptureMouse();
      _mouseDownPoint = e.GetPosition(AssociatedObject);

      var transform = GetRotateTransform();
      if (transform == null)
        return;

      _actualAngle = transform.Angle;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
      if (!_isRotating || !IsRotateEnabled)
        return;

      Point movePoint = e.GetPosition(AssociatedObject);

      double centerX = AssociatedObject.ActualWidth / 2;
      double centerY = AssociatedObject.ActualHeight / 2;

      double x1 = centerX;
      double y1 = centerY;
      double x2 = _mouseDownPoint.X;
      double y2 = _mouseDownPoint.Y;
      double x3 = centerX;
      double y3 = centerY;
      double x4 = movePoint.X;
      double y4 = movePoint.Y;

      double angle = Math.Atan2(y2 - y1, x2 - x1) - Math.Atan2(y4 - y3, x4 - x3);
      double result = angle * 180 / Math.PI;

      _actualAngle -= result;

      if (_actualAngle < 0)
        _actualAngle += 360;
      else if (_actualAngle > 360)
        _actualAngle -= 360;

      _mouseDownPoint = movePoint;

      Angle = SnapToPoint(_actualAngle);

      var transform = GetRotateTransform();
      if (transform == null)
        return;

      transform.Angle = Angle;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!IsRotateEnabled)
        return;

      if (_isRotating)
        AssociatedObject.ReleaseMouseCapture();

      _isRotating = false;
    }

    private double SnapToPoint(double angle)
    {

      if (!IsSnapEnabled || SnapPoints < 2)
        return angle;

      const double circle = 360;
      double snapInterval = circle / SnapPoints;

      // get the snap point
      double snapPoint = Math.Round(angle / snapInterval);
      // get snap angle
      double snapAngle = snapPoint * snapInterval;
      // start back at 0 when 360
      snapAngle = snapAngle % circle;

      return snapAngle;
    }

    protected RotateTransform GetRotateTransform()
    {
      if (string.IsNullOrWhiteSpace(RotateElement))
        return null;

      var element = AssociatedObject.FindName(RotateElement) as FrameworkElement;
      if (element == null)
        return null;

      // TODO create if not found?
      return element.RenderTransform as RotateTransform;
    }

  }
}
