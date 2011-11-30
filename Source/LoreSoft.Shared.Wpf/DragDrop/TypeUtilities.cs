﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LoreSoft.Shared.DragDrop
{
  public class TypeUtilities
  {
    public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
    {
      Type type = GetCommonBaseClass(source);
      Type listType = typeof(List<>).MakeGenericType(type);
      MethodInfo addMethod = listType.GetMethod("Add");
      object list = listType.GetConstructor(Type.EmptyTypes).Invoke(null);

      foreach (object o in source)
        addMethod.Invoke(list, new[] {o});

      return (IEnumerable)list;
    }

    public static Type GetCommonBaseClass(IEnumerable e)
    {
      var types = e.Cast<object>()
        .Select(o => o.GetType())
        .ToArray();

      return GetCommonBaseClass(types);
    }

    public static Type GetCommonBaseClass(Type[] types)
    {
      if (types.Length == 0)
        return typeof(object);

      Type baseType = types[0];

      for (int i = 1; i < types.Length; ++i)
      {
        if (!types[i].IsAssignableFrom(baseType))
          while (!baseType.IsAssignableFrom(types[i]))  // will terminate when ret == typeof(object)
            baseType = baseType.BaseType;
        else
          baseType = types[i];
      }

      return baseType;
    }
  }
}
