using System;
using System.Text;

namespace LoreSoft.Shared.Extensions
{
    public static class ExceptionExtensions
    {
        public static string Messages(this Exception exception)
        {
            var builder = new StringBuilder();

            Exception current = exception;
            while (current != null)
            {
                builder.Append(current.Message);
                if (current.InnerException != null)
                    builder.Append(" --> ");
                current = current.InnerException;
            }

            return builder.ToString();
        }
    }
}
