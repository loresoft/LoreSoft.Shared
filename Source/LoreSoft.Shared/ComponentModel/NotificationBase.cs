using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using LoreSoft.Shared.Messaging;
using LoreSoft.Shared.Reflection;

namespace LoreSoft.Shared.ComponentModel
{
    /// <summary>
    /// Base class for items that support property notification.
    /// </summary>
    /// <remarks>
    /// This class provides basic support for implementing the <see cref="INotifyPropertyChanged"/> interface and for
    /// marshalling execution to the UI thread.
    /// </remarks>
    [DataContract]
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class NotificationBase
      : DisposableBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>        
#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event for all of the properties.
        /// </summary>
        protected void RaisePropertyChanged()
        {
            RaisePropertyChanged((string)null);
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null)
                return;

            handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises this object's PropertyChanged event for each of the properties.
        /// </summary>
        /// <param name="propertyNames">The properties that have a new value.</param>
        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
                throw new ArgumentNullException("propertyNames");

            foreach (var name in propertyNames)
                RaisePropertyChanged(name);
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = ReflectionHelper.ExtractPropertyName(propertyExpression);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Sends the property changed message and raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        protected void SendPropertyChangedMessage<T>(Expression<Func<T>> propertyExpression)
        {
            SendPropertyChangedMessage<T>(propertyExpression, null, null);
        }

        /// <summary>
        /// Sends the property changed message and raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        /// <param name="oldValue">The old property value.</param>
        /// <param name="newValue">The new property value.</param>
        protected void SendPropertyChangedMessage<T>(Expression<Func<T>> propertyExpression, object oldValue, object newValue)
        {
            var propertyName = ReflectionHelper.ExtractPropertyName(propertyExpression);
            RaisePropertyChanged(propertyName);

            var message = new PropertyChangedMessage(this, propertyName, oldValue, newValue);
            Messenger.Current.PublishAsync(message);
        }

    }
}
