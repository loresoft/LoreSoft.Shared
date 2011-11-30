using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LoreSoft.Shared.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class PhoneNumberAttribute : RegularExpressionAttribute
    {
        // http://blog.stevenlevithan.com/archives/validate-phone-number
        public const string PhoneNumberRegex = "(?i)^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

        public PhoneNumberAttribute()
            : base(PhoneNumberRegex)
        {
            ErrorMessage = "Not a valid phone number.";
        }
    }
}
