using System;
using System.Collections.Generic;
using System.Linq;
//using NLog;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Diagnostics
{
    public static class Logger<TSource>
    {
        private static readonly Lazy<string> _loggerName = new Lazy<string>(() => typeof(TSource).FullName);
        public static string LoggerName
        {
            get { return _loggerName.Value; }
        }

        #region Debug
        public static Log Debug()
        {
            return new Log { LogLevel = LogLevel.Debug, LoggerName = LoggerName };
        }

        public static void Debug(string message)
        {
            Logger.Debug(LoggerName, message);
        }

        public static void Debug(Exception exception, string message)
        {
            Logger.Debug(LoggerName, exception, message);
        }

        public static void Debug(Exception exception, string format, params object[] args)
        {
            Logger.Debug(LoggerName, exception, format, args);
        }

        public static void Debug(string format, params object[] args)
        {
            Logger.Debug(LoggerName, format, args);
        }

        public static void Debug(IFormatProvider provider, string format, params object[] args)
        {
            Logger.Debug(LoggerName, provider, format, args);
        }
        #endregion

        #region Info
        public static Log Info()
        {
            return new Log { LogLevel = LogLevel.Info, LoggerName = LoggerName };
        }

        public static void Info(string message)
        {
            Logger.Info(LoggerName, message);
        }

        public static void Info(Exception exception, string message)
        {
            Logger.Info(LoggerName, exception, message);
        }

        public static void Info(Exception exception, string format, params object[] args)
        {
            Logger.Info(LoggerName, exception, format, args);
        }

        public static void Info(string format, params object[] args)
        {
            Logger.Info(LoggerName, format, args);
        }

        public static void Info(IFormatProvider provider, string format, params object[] args)
        {
            Logger.Info(LoggerName, provider, format, args);
        }
        #endregion

        #region Warn
        public static Log Warn()
        {
            return new Log { LogLevel = LogLevel.Warn, LoggerName = LoggerName };
        }

        public static void Warn(string message)
        {
            Logger.Warn(LoggerName, message);
        }

        public static void Warn(Exception exception, string message)
        {
            Logger.Warn(LoggerName, exception, message);
        }

        public static void Warn(Exception exception, string format, params object[] args)
        {
            Logger.Warn(LoggerName, exception, format, args);
        }

        public static void Warn(string format, params object[] args)
        {
            Logger.Warn(LoggerName, format, args);
        }

        public static void Warn(IFormatProvider provider, string format, params object[] args)
        {
            Logger.Warn(LoggerName, provider, format, args);
        }
        #endregion

        #region Error
        public static Log Error()
        {
            return new Log { LogLevel = LogLevel.Error, LoggerName = LoggerName };
        }

        public static void Error(string message)
        {
            Logger.Error(LoggerName, message);
        }

        public static void Error(Exception exception, string message)
        {
            Logger.Error(LoggerName, exception, message);
        }

        public static void Error(Exception exception, string format, params object[] args)
        {
            Logger.Error(LoggerName, exception, format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Logger.Error(LoggerName, format, args);
        }

        public static void Error(IFormatProvider provider, string format, params object[] args)
        {
            Logger.Error(LoggerName, provider, format, args);
        }
        #endregion

        public static void Log(ILogEvent eventInfo)
        {
            Logger.Log(LoggerName, eventInfo);
        }
    }

    public static class Logger
    {
        #region Debug
        public static Log Debug()
        {
            return new Log { LogLevel = LogLevel.Debug };
        }

        public static void Debug(object loggerName, string message)
        {
            //var log = GetLogger(loggerName);
            //log.Debug(message);
        }

        public static void Debug(object loggerName, Exception exception, string message)
        {
            //var log = GetLogger(loggerName);
            //log.DebugException(message, exception);
        }

        public static void Debug(object loggerName, Exception exception, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //var e = new LogEventInfo(LogLevel.Debug, log.Name, null, format, args, exception);
            //log.Log(e);
        }

        public static void Debug(object loggerName, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //log.Debug(format, args);
        }

        public static void Debug(object loggerName, IFormatProvider provider, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //log.Debug(provider, format, args);
        }
        #endregion

        #region Info
        public static Log Info()
        {
            return new Log { LogLevel = LogLevel.Info };
        }

        public static void Info(object loggerName, string message)
        {
            //var log = GetLogger(loggerName);
            //log.Info(message);
        }

        public static void Info(object loggerName, Exception exception, string message)
        {
            //var log = GetLogger(loggerName);
            //log.InfoException(message, exception);
        }

        public static void Info(object loggerName, Exception exception, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //var e = new LogEventInfo(LogLevel.Info, log.Name, null, format, args, exception);
            //log.Log(e);
        }

        public static void Info(object loggerName, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //log.Info(format, args);
        }

        public static void Info(object loggerName, IFormatProvider provider, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //log.Info(provider, format, args);
        }
        #endregion

        #region Warn
        public static Log Warn()
        {
            return new Log { LogLevel = LogLevel.Warn };
        }

        public static void Warn(object loggerName, string message)
        {
            //var log = GetLogger(loggerName);
            //log.Warn(message);
        }

        public static void Warn(object loggerName, Exception exception, string message)
        {
            //var log = GetLogger(loggerName);
            //log.WarnException(message, exception);
        }

        public static void Warn(object loggerName, Exception exception, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //var e = new LogEventInfo(LogLevel.Warn, log.Name, null, format, args, exception);
            //log.Log(e);
        }

        public static void Warn(object loggerName, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //log.Warn(format, args);
        }

        public static void Warn(object loggerName, IFormatProvider provider, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //log.Warn(provider, format, args);
        }
        #endregion

        #region Error
        public static Log Error()
        {
            return new Log { LogLevel = LogLevel.Error };
        }

        public static void Error(object loggerName, string message)
        {
            //var log = GetLogger(loggerName);
            //log.Error(message);
        }

        public static void Error(object loggerName, Exception exception, string message)
        {
            //var log = GetLogger(loggerName);
            //log.ErrorException(message, exception);
        }

        public static void Error(object loggerName, Exception exception, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //var e = new LogEventInfo(LogLevel.Error, log.Name, null, format, args, exception);
            //log.Log(e);
        }

        public static void Error(object loggerName, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //log.Error(format, args);
        }

        public static void Error(object loggerName, IFormatProvider provider, string format, params object[] args)
        {
            //var log = GetLogger(loggerName);
            //log.Error(provider, format, args);
        }
        #endregion

        public static void Log(object loggerName, ILogEvent eventInfo)
        {
            //var log = GetLogger(loggerName);
            //eventInfo.LoggerName = log.Name;

            //log.Log(eventInfo);
        }

        internal static ILogger GetLogger(object name)
        {
            //var loggerName = GetLoggerName(name);
            //return LogManager.GetLogger(loggerName);
            return null;
        }

        internal static string GetLoggerName(object name)
        {
            if (name == null)
                return "Logger";

            if (name is string)
                return name as string;

            if (name is Type)
                return (name as Type).FullName;

            return name.GetType().FullName;
        }
    }

    public class Log
    {
        public Log()
        {
            _parameters = new List<object>();
            _properties = new Dictionary<object, object>();
            LogLevel = Diagnostics.LogLevel.Debug;
            LoggerName = "Logger";
        }

        /// <summary>
        /// Gets or sets the level of the logging event.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the logger name.
        /// </summary>
        public string LoggerName { get; set; }

        /// <summary>
        /// Gets or sets the log message including any parameter placeholders.
        /// </summary>
        public string Message { get; set; }

        private readonly IList<object> _parameters;
        /// <summary>
        /// Gets or sets the parameter values or null if no parameters have been specified.
        /// </summary>
        public IList<object> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Gets or sets the format provider that was provided while logging or <see langword="null" />
        /// when no formatProvider was specified.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the exception information.
        /// </summary>
        public Exception Exception { get; set; }

        private readonly IDictionary<object, object> _properties;

        /// <summary>
        /// Gets the dictionary of per-event context properties.
        /// </summary>
        public IDictionary<object, object> Properties
        {
            get { return _properties; }
        }

        public Log WithMessage(string message)
        {
            Message = message;
            return this;
        }

        public Log WithParameter(params object[] parameters)
        {
            Parameters.AddRange(parameters);
            return this;
        }

        public Log WithException(Exception ex)
        {
            Exception = ex;
            return this;
        }

        public Log WithLogger(string loggerName)
        {
            LoggerName = loggerName;
            return this;
        }

        public Log WithLogger(object logger)
        {
            LoggerName = Logger.GetLoggerName(logger);
            return this;
        }

        public Log WithLogger<T>()
        {
            LoggerName = typeof(T).FullName;
            return this;
        }

        public Log WithProperty(object key, object value)
        {
            Properties[key] = value;
            return this;
        }

        public Log WithFormatProvider(IFormatProvider formatProvider)
        {
            FormatProvider = formatProvider;
            return this;
        }

        public Log Write()
        {
            //var eventInfo = new LogEventInfo();
            //eventInfo.LoggerName = LoggerName;
            //eventInfo.Level = NLog.LogLevel.FromOrdinal((int)LogLevel);
            //eventInfo.Message = Message;
            //eventInfo.Parameters = Parameters.ToArray();
            //eventInfo.Exception = Exception;
            //eventInfo.FormatProvider = FormatProvider;

            //foreach (var property in Properties)
            //  eventInfo.Properties[property.Key] = property.Value;

            //Logger.Log(LoggerName, eventInfo);

            return this;
        }
    }
}
