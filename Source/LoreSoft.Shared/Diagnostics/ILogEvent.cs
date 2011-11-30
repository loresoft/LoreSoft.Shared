using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LoreSoft.Shared.Diagnostics
{
    public interface ILogEvent
    {
        DateTime TimeStamp { get; set; }

        LogLevel Level { get; set; }

        Exception Exception { get; set; }

        string LoggerName { get; set; }

        string Message { get; set; }

        object[] Parameters { get; set; }

        IFormatProvider FormatProvider { get; set; }

        string FormattedMessage { get; }

        IDictionary<object, object> Properties { get; set; }
    }
}