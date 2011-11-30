using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LoreSoft.Shared.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class CreditCardAttribute : ValidationAttribute
    {
        private readonly object _syncLock = new object();

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

                if (IsCardNumberValid(convertedValue))
                    return ValidationResult.Success;

                var memberNames = (validationContext.MemberName != null) ? new[] { validationContext.MemberName } : null;
                return new ValidationResult(
                  FormatErrorMessage(validationContext.DisplayName),
                  memberNames);
            }
        }

        /// <summary>
        /// Validates a credit card number using the standard Luhn/mod10 validation algorithm.
        /// </summary>
        /// <param name="cardNumber">Card number, with or without punctuation</param>
        /// <returns>
        /// 	<c>true</c> if card number appears valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCardNumberValid(string cardNumber)
        {
            int i;

            var cleanNumber = new StringBuilder();
            for (i = 0; i < cardNumber.Length; i++)
                if (char.IsDigit(cardNumber, i))
                    cleanNumber.Append(cardNumber[i]);

            if (cleanNumber.Length < 13 || cleanNumber.Length > 16)
                return false;

            for (i = cleanNumber.Length + 1; i <= 16; i++)
                cleanNumber.Insert(0, "0");

            string number = cleanNumber.ToString();
            int total = 0;

            for (i = 1; i <= 16; i++)
            {
                int multiplier = 1 + (i % 2);
                int digit = int.Parse(number.Substring(i - 1, 1));
                int sum = digit * multiplier;

                if (sum > 9)
                    sum -= 9;

                total += sum;
            }
            return (total % 10 == 0);
        }

        /// <summary>
        /// Cleans the credit card number.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <returns>The card number with only the valid digits.</returns>
        public static string FormatCardNumber(string cardNumber)
        {
            var cleanNumber = new StringBuilder();
            for (int i = 0; i < cardNumber.Length; i++)
                if (char.IsDigit(cardNumber, i))
                    cleanNumber.Append(cardNumber[i]);

            return cleanNumber.ToString();
        }

        /// <summary>
        /// Determines whether the credit card is exired.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <returns>
        /// 	<c>true</c> if credit card is exired; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCardExired(int year, int month)
        {
            // last day of the month
            var expireDate = new DateTime(year, month, 1)
                .AddMonths(1)
                .Subtract(TimeSpan.FromDays(1));

            return (expireDate < DateTime.Today);
        }
    }
}
