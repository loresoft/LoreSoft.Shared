using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

#if SILVERLIGHT
using DispatchSource = System.Windows.DependencyObject;
#else
using DispatchSource = System.Windows.Threading.DispatcherObject;
#endif

namespace LoreSoft.Shared.Extensions
{
  public static class DispatcherExtensions
  {
    #region DispatcherObject
#if !SILVERLIGHT
    public static TResult Dispatch<TResult>(this DispatchSource source, Func<TResult> func)
    {
      if (source.Dispatcher.CheckAccess())
        return func();

      return (TResult)source.Dispatcher.Invoke(func);
    }

    public static TResult Dispatch<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
      where TSource : DispatchSource
    {
      if (source.Dispatcher.CheckAccess())
        return func(source);

      return (TResult)source.Dispatcher.Invoke(func, source);
    }

    public static TResult Dispatch<TSource, T, TResult>(this TSource source, Func<TSource, T, TResult> func, T param1)
      where TSource : DispatchSource
    {
      if (source.Dispatcher.CheckAccess())
        return func(source, param1);

      return (TResult)source.Dispatcher.Invoke(func, source, param1);
    }

    public static TResult Dispatch<TSource, T1, T2, TResult>(this TSource source, Func<TSource, T1, T2, TResult> func, T1 param1, T2 param2)
      where TSource : DispatchSource
    {
      if (source.Dispatcher.CheckAccess())
        return func(source, param1, param2);

      return (TResult)source.Dispatcher.Invoke(func, source, param1, param2);
    }

    public static TResult Dispatch<TSource, T1, T2, T3, TResult>(this TSource source, Func<TSource, T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3)
      where TSource : DispatchSource
    {
      if (source.Dispatcher.CheckAccess())
        return func(source, param1, param2, param3);

      return (TResult)source.Dispatcher.Invoke(func, source, param1, param2, param3);
    }
#endif

    public static void Dispatch(this DispatchSource source, Action func)
    {
      if (source.Dispatcher.CheckAccess())
        func();
      else
        source.Dispatcher.BeginInvoke(func);
    }

    public static void Dispatch<TSource>(this TSource source, Action<TSource> func)
      where TSource : DispatchSource
    {
      if (source.Dispatcher.CheckAccess())
        func(source);
      else
        source.Dispatcher.BeginInvoke(func, source);
    }

    public static void Dispatch<TSource, T1>(this TSource source, Action<TSource, T1> func, T1 param1)
      where TSource : DispatchSource
    {
      if (source.Dispatcher.CheckAccess())
        func(source, param1);
      else
        source.Dispatcher.BeginInvoke(func, source, param1);
    }

    public static void Dispatch<TSource, T1, T2>(this TSource source, Action<TSource, T1, T2> func, T1 param1, T2 param2)
      where TSource : DispatchSource
    {
      if (source.Dispatcher.CheckAccess())
        func(source, param1, param2);
      else
        source.Dispatcher.BeginInvoke(func, source, param1, param2);
    }

    public static void Dispatch<TSource, T1, T2, T3>(this TSource source, Action<TSource, T1, T2, T3> func, T1 param1, T2 param2, T3 param3)
      where TSource : DispatchSource
    {
      if (source.Dispatcher.CheckAccess())
        func(source, param1, param2, param3);
      else
        source.Dispatcher.BeginInvoke(func, source, param1, param2, param3);
    }
    #endregion

    #region Dispatcher
#if !SILVERLIGHT
    public static void DoEvents(this Dispatcher dispatcher)
    {
      var frame = new DispatcherFrame();

      dispatcher.BeginInvoke(
        DispatcherPriority.Background,
        new Action<DispatcherFrame>(f => f.Continue = false),
        frame);

      Dispatcher.PushFrame(frame);
    }

    public static TResult Dispatch<TResult>(this Dispatcher dispatcher, Func<TResult> func)
    {
      if (dispatcher.CheckAccess())
        return func();

      return (TResult)dispatcher.Invoke(func);
    }

    public static TResult Dispatch<T, TResult>(this Dispatcher dispatcher, Func<T, TResult> func, T param1)
    {
      if (dispatcher.CheckAccess())
        return func(param1);

      return (TResult)dispatcher.Invoke(func, param1);
    }

    public static TResult Dispatch<T1, T2, TResult>(this Dispatcher dispatcher, Func<T1, T2, TResult> func, T1 param1, T2 param2)
    {
      if (dispatcher.CheckAccess())
        return func(param1, param2);

      return (TResult)dispatcher.Invoke(func, param1, param2);
    }

    public static TResult Dispatch<T1, T2, T3, TResult>(this Dispatcher dispatcher, Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3)
    {
      if (dispatcher.CheckAccess())
        return func(param1, param2, param3);

      return (TResult)dispatcher.Invoke(func, param1, param2, param3);
    }
#endif

    public static void Dispatch(this Dispatcher dispatcher, Action func)
    {
      if (dispatcher.CheckAccess())
        func();
      else
        dispatcher.BeginInvoke(func);
    }

    public static void Dispatch<T1>(this Dispatcher dispatcher, Action<T1> func, T1 param1)
    {
      if (dispatcher.CheckAccess())
        func(param1);
      else
        dispatcher.BeginInvoke(func, param1);
    }

    public static void Dispatch<T1, T2>(this Dispatcher dispatcher, Action<T1, T2> func, T1 param1, T2 param2)
    {
      if (dispatcher.CheckAccess())
        func(param1, param2);
      else
        dispatcher.BeginInvoke(func, param1, param2);
    }

    public static void Dispatch<T1, T2, T3>(this Dispatcher dispatcher, Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3)
    {
      if (dispatcher.CheckAccess())
        func(param1, param2, param3);
      else
        dispatcher.BeginInvoke(func, param1, param2, param3);
    }


    #endregion

    /// <summary>
    /// Gets the Visual Tree filtered by Type for a DependencyObject with that DependencyObject as the root.
    /// </summary>
    public static IEnumerable<T> GetVisualTree<T>(this DependencyObject element)
    {
      return GetVisualTree(element)
        .Where(t => typeof(T).IsAssignableFrom(t.GetType()))
        .Cast<T>();
    }

    /// <summary>
    /// Gets the Visual Tree for a DependencyObject with that DependencyObject as the root.
    /// </summary>
    public static IEnumerable<DependencyObject> GetVisualTree(this DependencyObject element)
    {
      var contentControl = element as ContentControl;
      if (contentControl != null && contentControl.Content != null)
      {
        var content = contentControl.Content as DependencyObject;
        if (content != null)
        {
          yield return content;
          foreach (var dependencyObject in GetVisualTree(content))
            yield return dependencyObject;
        }
      }

      var itemsControl = element as ItemsControl;
      if (itemsControl != null && itemsControl.Items != null)
      {
        foreach (var item in itemsControl.Items.OfType<DependencyObject>())
        {
          yield return item;
          foreach (var dependencyObject in GetVisualTree(item))
            yield return dependencyObject;
        }
      }

      int childrenCount = VisualTreeHelper.GetChildrenCount(element);
      for (int i = 0; i < childrenCount; i++)
      {
        var visualChild = VisualTreeHelper.GetChild(element, i);
        yield return visualChild;
        foreach (var visualChildren in GetVisualTree(visualChild))
          yield return visualChildren;
      }
    }
  }
}
