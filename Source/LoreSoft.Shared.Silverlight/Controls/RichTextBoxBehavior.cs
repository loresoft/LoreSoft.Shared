using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LoreSoft.Shared.Controls
{
  public class RichTextBoxBehavior : Behavior<RichTextBox>
  {   
    #region Block
    public object Block
    {
      get { return GetValue(BlockProperty); }
      set { SetValue(BlockProperty, value); }
    }

    public static readonly DependencyProperty BlockProperty =
        DependencyProperty.Register(
            "Block",
            typeof(object),
            typeof(RichTextBoxBehavior),
            new PropertyMetadata(null, OnBlockChanged));

    private static void OnBlockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as RichTextBoxBehavior;
      if (source == null)
        return;

      source.OnBlockChanged(e);
    }

    protected virtual void OnBlockChanged(DependencyPropertyChangedEventArgs e)
    {
      UpadateBlocks();
    }
    #endregion

    protected override void OnAttached()
    {
      UpadateBlocks();
    }

    private void UpadateBlocks()
    {
      if (AssociatedObject == null || AssociatedObject.Blocks == null)
        return;
      
      AssociatedObject.Blocks.Clear();
      if (Block == null)
        return;

      var b = Block as Block;
      if (b == null)
        return;

      AssociatedObject.Blocks.Add(b);
    }
  }
}

