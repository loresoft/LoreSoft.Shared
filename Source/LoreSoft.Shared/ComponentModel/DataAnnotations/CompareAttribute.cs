using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace LoreSoft.Shared.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CompareAttribute : ValidationAttribute
    {
        private object _syncLock = new object();

        public CompareAttribute(string otherProperty)
            : base("'{0}' and '{1}' do not match.")
        {
            if (otherProperty == null)
            {
                throw new ArgumentNullException("otherProperty");
            }
            OtherProperty = otherProperty;
        }

        public string OtherProperty { get; private set; }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            lock (_syncLock)
            {
                PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
                var memberNames = (validationContext.MemberName != null) ? new[] { validationContext.MemberName } : null;

                if (otherPropertyInfo == null)
                    return new ValidationResult(
                      string.Format("Could not find a property named {0}.", OtherProperty),
                      memberNames);

                object otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

                if (Equals(value, otherPropertyValue))
                    return ValidationResult.Success; ;

                return new ValidationResult(
                  FormatErrorMessage(validationContext.DisplayName),
                  memberNames);
            }
        }
    }
}
