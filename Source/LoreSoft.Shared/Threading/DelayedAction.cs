using System;
using System.Threading;
using LoreSoft.Shared.ComponentModel;

namespace LoreSoft.Shared.Threading
{
    /// <summary>
    /// A class representing a sliding delayed action
    /// </summary>
    public sealed class DelayedAction : DisposableBase
    {
        private readonly Timer _delayTimer;
        private readonly TimeSpan _infinite = TimeSpan.FromMilliseconds(Timeout.Infinite);
        private readonly SynchronizationContext _capturedContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedAction"/> class.
        /// </summary>
        /// <param name="action">The action to perform after delay.</param>
        /// <param name="delay">The time to delay before invoking the action.</param>
        public DelayedAction(Action action, TimeSpan delay)
        {
            _capturedContext = SynchronizationContext.Current;
            _delayTimer = new Timer(Invoke);
            _delay = delay;
            _action = action;
        }

        private readonly TimeSpan _delay;
        /// <summary>
        /// Gets the time to delay before invoking the action.
        /// </summary>
        public TimeSpan Delay
        {
            get { return _delay; }
        }

        private readonly Action _action;
        /// <summary>
        /// Gets the action to perform after delay.
        /// </summary>
        public Action Action
        {
            get { return _action; }
        }

        /// <summary>
        /// Cancels the start of delayed action.
        /// </summary>
        public void Cancel()
        {
            _delayTimer.Change(_infinite, _infinite);
        }

        /// <summary>
        /// Triggers the start of delayed action. Repeated calls will act like a sliding delay.
        /// </summary>
        public void Trigger()
        {
            _delayTimer.Change(_delay, _infinite);
        }

        /// <summary>
        /// Triggers the start of delayed action. Repeated calls will act like a sliding delay.
        /// </summary>
        /// <param name="delay">The time to delay before invoking the action.</param>
        public void Trigger(TimeSpan delay)
        {
            _delayTimer.Change(delay, _infinite);
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _delayTimer.Dispose();
        }

        private void Invoke(object state)
        {
            if (_capturedContext != null)
                _capturedContext.Post(o => _action(), null);
            else
                _action();
        }
    }
}
