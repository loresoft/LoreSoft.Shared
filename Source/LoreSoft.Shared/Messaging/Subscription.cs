using System;

namespace LoreSoft.Shared.Messaging
{
    /// <summary>
    /// A class that holds a message subscription.
    /// </summary>
    internal class Subscription
    {
        private readonly string _messageKey;
        private readonly SubscriptionToken _token;
        private readonly WeakAction _weakAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="action">The delegate action to create a WeakAction from.</param>
        /// <param name="actionType">The parameter type of the action.</param>
        public Subscription(Delegate action, Type actionType)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (actionType == null)
                throw new ArgumentNullException("actionType");

            _token = new SubscriptionToken();
            _messageKey = actionType.AssemblyQualifiedName;
            _weakAction = new WeakAction(action.Target, action.Method, actionType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="messageKey">The message key.</param>
        /// <param name="action">The delegate action to create a WeakAction from.</param>
        /// <param name="actionType">The parameter type of the action.</param>
        public Subscription(string messageKey, Delegate action, Type actionType)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (actionType == null)
                throw new ArgumentNullException("actionType");

            _token = new SubscriptionToken();
            _messageKey = messageKey;
            _weakAction = new WeakAction(action.Target, action.Method, actionType);
        }

        /// <summary>
        /// Gets the message key. The default message key is the message type full name.
        /// </summary>
        public string MessageKey
        {
            get { return _messageKey; }
        }

        /// <summary>
        /// Gets the token used for unsubscribing this subscription.
        /// </summary>
        public SubscriptionToken Token
        {
            get { return _token; }

        }

        /// <summary>
        /// Gets the weak reference action for this subscription.
        /// </summary>
        public WeakAction WeakAction
        {
            get { return _weakAction; }
        }

        /// <summary>
        /// Creates the delegate from the <see cref="WeakAction"/>.
        /// </summary>
        /// <returns>
        /// A new instance of a Delegate created from the <see cref="WeakAction"/>.  
        /// If the Target of the <see cref="WeakAction"/> was disposed, <c>null</c> is returned.
        /// </returns>
        internal Delegate CreateDelegate()
        {
            if (_weakAction == null)
                return null;

            return _weakAction.CreateDelegate();
        }
    }
}