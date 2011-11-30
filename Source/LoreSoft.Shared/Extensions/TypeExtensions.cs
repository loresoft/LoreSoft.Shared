using System;

namespace LoreSoft.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            if (type.IsValueType)
                return false;

            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static object Default(this Type type)
        {
            return type.IsValueType
              ? Activator.CreateInstance(type)
              : null;
        }
    }
}
