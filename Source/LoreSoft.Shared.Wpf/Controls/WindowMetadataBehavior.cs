using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using LoreSoft.Shared.Interop;
using LoreSoft.Shared.Threading;

namespace LoreSoft.Shared.Controls
{
  public class WindowMetadataBehavior : Behavior<System.Windows.Window>
  {
    private readonly BusyMonitor _saveMonitor;
    private readonly BusyMonitor _loadMonitor;
    private bool _isLoaded;

    public WindowMetadataBehavior()
    {
      _saveMonitor = new BusyMonitor();
      _loadMonitor = new BusyMonitor();
    }

    #region Metadata
    public WindowMetadata Metadata
    {
      get { return (WindowMetadata)GetValue(MetadataProperty); }
      set { SetValue(MetadataProperty, value); }
    }

    public static readonly DependencyProperty MetadataProperty =
        DependencyProperty.Register(
            "Metadata",
            typeof(WindowMetadata),
            typeof(WindowMetadataBehavior),
            new PropertyMetadata(null, OnMetadataChanged));

    private static void OnMetadataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as WindowMetadataBehavior;
      if (source == null)
        return;

      source.OnMetadataChanged(e);
    }

    protected virtual void OnMetadataChanged(DependencyPropertyChangedEventArgs e)
    {
      if (_saveMonitor.IsBusy || !_isLoaded)
        return;

      LoadWindowState();
    }
    #endregion
    
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.SourceInitialized += OnWindowSourceInitialized;
      AssociatedObject.Closing += OnWindowClosing;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();

      AssociatedObject.SourceInitialized -= OnWindowSourceInitialized;
      AssociatedObject.Closing -= OnWindowClosing;
    }

    private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (_loadMonitor.IsBusy)
        return;

      SaveWindowState();
    }

    private void OnWindowSourceInitialized(object sender, EventArgs e)
    {
      _isLoaded = true;
      LoadWindowState();
    }
    
    private void LoadWindowState()
    {
      if (Metadata == null)
        return;

      using (_loadMonitor.Enter())
      {
        var wp = Metadata.ToPlacement();
        var helper = new WindowInteropHelper(AssociatedObject);
        NativeMethods.SetWindowPlacement(helper.Handle, wp);
      }
    }

    private void SaveWindowState()
    {
      var helper = new WindowInteropHelper(AssociatedObject);
      var wp = NativeMethods.GetWindowPlacement(helper.Handle);
      var metadata = new WindowMetadata(wp);

      if (Metadata != null && Metadata == metadata)
        return;

      using (_saveMonitor.Enter())
        Metadata = metadata;
    }
  }

  [DataContract]
  public class WindowMetadata : IEquatable<WindowMetadata>
  {
    public WindowMetadata()
    { }

    internal WindowMetadata(WINDOWPLACEMENT placement)
    {
      ShowCommand = placement.showCmd;
      MinimizedX = placement.minPosition.X;
      MinimizedY = placement.minPosition.Y;
      MaximizedX = placement.maxPosition.X;
      MaximizedY = placement.maxPosition.Y;
      NormalLeft = placement.normalPosition.Left;
      NormalTop = placement.normalPosition.Top;
      NormalRight = placement.normalPosition.Right;
      NormalBottom = placement.normalPosition.Bottom;
    }

    internal WINDOWPLACEMENT ToPlacement()
    {
      var placement = new WINDOWPLACEMENT();
      placement.showCmd = ShowCommand;
      placement.minPosition.X = MinimizedX;
      placement.minPosition.Y = MinimizedY;
      placement.maxPosition.X = MaximizedX;
      placement.maxPosition.Y = MaximizedY;
      placement.normalPosition.Left = NormalLeft;
      placement.normalPosition.Top = NormalTop;
      placement.normalPosition.Right = NormalRight;
      placement.normalPosition.Bottom = NormalBottom;

      return placement;
    }

    [DataMember]
    public int ShowCommand { get; set; }

    [DataMember]
    public int MinimizedX { get; set; }
    [DataMember]
    public int MinimizedY { get; set; }

    [DataMember]
    public int MaximizedX { get; set; }
    [DataMember]
    public int MaximizedY { get; set; }

    [DataMember]
    public int NormalLeft { get; set; }
    [DataMember]
    public int NormalTop { get; set; }
    [DataMember]
    public int NormalRight { get; set; }
    [DataMember]
    public int NormalBottom { get; set; }

    public bool Equals(WindowMetadata other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return other.ShowCommand == ShowCommand 
        && other.MinimizedX == MinimizedX 
        && other.MinimizedY == MinimizedY 
        && other.MaximizedX == MaximizedX 
        && other.MaximizedY == MaximizedY 
        && other.NormalLeft == NormalLeft 
        && other.NormalTop == NormalTop 
        && other.NormalRight == NormalRight 
        && other.NormalBottom == NormalBottom;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != typeof (WindowMetadata))
        return false;

      return Equals((WindowMetadata)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = ShowCommand;
        result = (result * 397) ^ MinimizedX;
        result = (result * 397) ^ MinimizedY;
        result = (result * 397) ^ MaximizedX;
        result = (result * 397) ^ MaximizedY;
        result = (result * 397) ^ NormalLeft;
        result = (result * 397) ^ NormalTop;
        result = (result * 397) ^ NormalRight;
        result = (result * 397) ^ NormalBottom;
        return result;
      }
    }

    public static bool operator ==(WindowMetadata left, WindowMetadata right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(WindowMetadata left, WindowMetadata right)
    {
      return !Equals(left, right);
    }
  }
}
