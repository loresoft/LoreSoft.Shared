using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace LoreSoft.Shared
{
    public sealed class EnumItem : IEquatable<EnumItem>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object UnderlyingValue { get; set; }
        public object OriginalValue { get; set; }

        public override string ToString()
        {
            return Description;
        }

        public bool Equals(EnumItem other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Equals(other.UnderlyingValue, UnderlyingValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is EnumItem)
                return Equals((EnumItem)obj);

            return Equals(obj, UnderlyingValue);
        }

        public override int GetHashCode()
        {
            return (UnderlyingValue != null ? UnderlyingValue.GetHashCode() : 0);
        }

        public static IEnumerable<EnumItem> Create<T>()
          where T : struct, IComparable, IFormattable, IConvertible
        {
            Type enumType = typeof(T);
            return CreateList(enumType);
        }

        public static EnumItem Create(object enumValue)
        {
            Type enumType = enumValue.GetType();
            string name = Enum.GetName(enumType, enumValue);

            Type underlyingType = Enum.GetUnderlyingType(enumType);
            object underlyingValue = Convert.ChangeType(enumValue, underlyingType, Thread.CurrentThread.CurrentCulture);

            return new EnumItem
            {
                Name = name,
                Description = name,
                UnderlyingValue = underlyingValue,
                OriginalValue = enumValue
            };
        }

        public static IEnumerable<EnumItem> CreateList(Type enumType)
        {
            return from field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                   let attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .OfType<DescriptionAttribute>()
                    .FirstOrDefault()
                   let description = attribute == null ? field.Name : attribute.Description
                   let value = field.GetRawConstantValue()
                   let original = Enum.ToObject(enumType, value)
                   select new EnumItem
                   {
                       Name = field.Name,
                       Description = description,
                       UnderlyingValue = value,
                       OriginalValue = original
                   };
        }
    }
}