using System;
using System.Diagnostics;

namespace LoreSoft.Shared.ComponentModel
{
    /// <summary>
    /// A class representing a singleton pattern.
    /// </summary>
    /// <typeparam name="T">The type of the singleton</typeparam>
    public abstract class SingletonBase<T>
      : NotificationBase where T : SingletonBase<T>, new()
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected SingletonBase()
        { }

        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());

        /// <summary>
        /// Gets the current instance of the singleton.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DebuggerNonUserCode]
        public static T Current
        {
            get { return _instance.Value; }
        }
    }
}
