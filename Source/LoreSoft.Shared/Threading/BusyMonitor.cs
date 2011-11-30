using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LoreSoft.Shared.Threading
{
    /// <summary>
    /// A class to monitor if an action is busy.
    /// </summary>
    public class BusyMonitor
    {
        private int _busyCount;

        /// <summary>
        /// Exits this monitor and decrements the busy count.
        /// </summary>
        public void Exit()
        {
            Interlocked.Decrement(ref _busyCount);
        }

        /// <summary>
        /// Enters this monitor and increments the busy count.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> instance that calls <see cref="Exit"/> when disposed.</returns>
        public IDisposable Enter()
        {
            Interlocked.Increment(ref _busyCount);
            return new DisposeAction(Exit);
        }

        /// <summary>
        /// Gets a value indicating whether this monitor is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this monitor is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get { return (_busyCount > 0); }
        }

        /// <summary>
        /// Gets the busy count.
        /// </summary>
        public int BusyCount
        {
            get { return _busyCount; }
        }

        private class DisposeAction : IDisposable
        {
            private readonly Action _exitAction;

            public DisposeAction(Action exitAction)
            {
                _exitAction = exitAction;
            }

            void IDisposable.Dispose()
            {
                _exitAction.Invoke();
            }
        }
    }
}
