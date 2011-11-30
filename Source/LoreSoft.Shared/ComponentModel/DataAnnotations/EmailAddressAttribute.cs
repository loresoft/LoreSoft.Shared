using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LoreSoft.Shared.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class EmailAddressAttribute : ValidationAttribute
    {
        private object _syncLock = new object();

        public EmailAddressAttribute()
            : this("Not a valid email address.")
        { }

        public EmailAddressAttribute(string errorMessage)
            : base(errorMessage)
        { }

        public EmailAddressAttribute(Func<string> errorMessageAccessor)
            : base(errorMessageAccessor)
        { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            lock (_syncLock)
            {
                if (value == null)
                    return ValidationResult.Success;

                string convertedValue;
                try
                {
                    convertedValue = Convert.ToString(value);
                }
                catch (FormatException)
                {
                    return new ValidationResult("Failed to convert value to string.");
                }

                var emailParser = new EmailAddressValidator();
                if (emailParser.IsEmailValid(convertedValue))
                    return ValidationResult.Success;

                var memberNames = (validationContext.MemberName != null) ? new[] { validationContext.MemberName } : null;
                return new ValidationResult(
                  FormatErrorMessage(validationContext.DisplayName),
                  memberNames);
            }
        }

    }
}

