namespace LoreSoft.Shared.Threading
{
    /// <summary>
    /// Synchronized access wrapper class
    /// </summary>
    /// <typeparam name="T">The type that has its access synchronized.</typeparam>
    public class Synchronized<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LoreSoft.Shared.Threading.Synchronized`1"/> class.
        /// </summary>
        public Synchronized()
            : this(default(T), new object())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LoreSoft.Shared.Threading.Synchronized`1"/> class.
        /// </summary>
        /// <param name="value">The initial value.</param>
        public Synchronized(T value)
            : this(value, new object())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LoreSoft.Shared.Threading.Synchronized`1"/> class.
        /// </summary>
        /// <param name="value">The initial value.</param>
        /// <param name="valueLock">The shared lock.</param>
        public Synchronized(T value, object valueLock)
        {
            _valueLock = valueLock ?? new object();
            Value = value;
        }

        private readonly object _valueLock;
        private T _value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get
            {
                lock (_valueLock)
                    return _value;
            }
            set
            {
                lock (_valueLock)
                    _value = value;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:LoreSoft.Shared.Threading.Synchronized`1"/> to <see cref="T"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator T(Synchronized<T> value)
        {
            return value.Value;
        }

    }
}
