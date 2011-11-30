#if !SILVERLIGHT

using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoreSoft.Shared.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>An already canceled task.</summary>
        internal static Lazy<Task> CanceledTask = new Lazy<Task>(() =>
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            taskCompletionSource.TrySetCanceled();
            return taskCompletionSource.Task;
        });
        /// <summary>An already canceled task.</summary>
        internal static Lazy<Task> CompletedTask = new Lazy<Task>(() =>
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            taskCompletionSource.TrySetResult(null);
            return taskCompletionSource.Task;
        });


        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        public static Task WithTimeout(this Task task, TimeSpan timeout)
        {
            var result = new TaskCompletionSource<object>(task.AsyncState);
            var timer = new Timer(state =>
            {
                var source = state as TaskCompletionSource<object>;
                source.TrySetCanceled();
            }, result, timeout, TimeSpan.FromMilliseconds(-1));

            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            }, TaskContinuationOptions.ExecuteSynchronously);

            return result.Task;
        }

        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <typeparam name="TResult">Specifies the type of data contained in the task.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        public static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var result = new TaskCompletionSource<TResult>(task.AsyncState);
            var timer = new Timer(state =>
            {
                var source = state as TaskCompletionSource<TResult>;
                source.TrySetCanceled();
            }, result, timeout, TimeSpan.FromMilliseconds(-1));

            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            }, TaskContinuationOptions.ExecuteSynchronously);

            return result.Task;
        }
    }

    public static class TaskCompletionSourceExtensions
    {
        /// <summary>Transfers the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="resultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion: resultSetter.SetResult(task is Task<TResult> ? ((Task<TResult>)task).Result : default(TResult)); break;
                case TaskStatus.Faulted: resultSetter.SetException(task.Exception.InnerExceptions); break;
                case TaskStatus.Canceled: resultSetter.SetCanceled(); break;
                default: throw new InvalidOperationException("The task was not completed.");
            }
        }

        /// <summary>Transfers the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="resultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task<TResult> task)
        {
            SetFromTask(resultSetter, (Task)task);
        }

        /// <summary>Attempts to transfer the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="resultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        /// <returns>Whether the transfer could be completed.</returns>
        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion: return resultSetter.TrySetResult(task is Task<TResult> ? ((Task<TResult>)task).Result : default(TResult));
                case TaskStatus.Faulted: return resultSetter.TrySetException(task.Exception.InnerExceptions);
                case TaskStatus.Canceled: return resultSetter.TrySetCanceled();
                default: throw new InvalidOperationException("The task was not completed.");
            }
        }

        /// <summary>Attempts to transfer the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="resultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        /// <returns>Whether the transfer could be completed.</returns>
        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task<TResult> task)
        {
            return TrySetFromTask(resultSetter, (Task)task);
        }
    }

    public static class TaskFactoryExtensions
    {
        #region TaskFactory No Action
        /// <summary>Creates a Task that will complete after the specified delay.</summary>
        /// <param name="factory">The TaskFactory.</param>
        /// <param name="dueTime">The delay after which the Task should transition to RanToCompletion.</param>
        /// <returns>A Task that will be completed after the specified duration.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime)
        {
            return StartNewDelayed(factory, dueTime, CancellationToken.None);
        }

        /// <summary>Creates a Task that will complete after the specified delay.</summary>
        /// <param name="factory">The TaskFactory.</param>
        /// <param name="dueTime">The delay after which the Task should transition to RanToCompletion.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel the timed task.</param>
        /// <returns>A Task that will be completed after the specified duration and that's cancelable with the specified token.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime, CancellationToken cancellationToken)
        {
            // Validate arguments
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (dueTime < -1)
                throw new ArgumentOutOfRangeException("dueTime");
            if (dueTime == 0)
                return TaskExtensions.CompletedTask.Value;

            // Check for a pre-canceled token
            //if (cancellationToken.IsCancellationRequested)
            //  return factory.FromCancellation(cancellationToken);

            // Create the timed task
            var tcs = new TaskCompletionSource<object>(factory.CreationOptions);
            var ctr = default(CancellationTokenRegistration);

            // Create the timer but don't start it yet.  If we start it now,
            // it might fire before ctr has been set to the right registration.
            var timer = new Timer(self =>
            {
                // Clean up both the cancellation token and the timer, and try to transition to completed
                ctr.Dispose();
                ((Timer)self).Dispose();
                tcs.TrySetResult(null);
            });

            // Register with the cancellation token.
            if (cancellationToken.CanBeCanceled)
            {
                // When cancellation occurs, cancel the timer and try to transition to canceled.
                // There could be a race, but it's benign.
                ctr = cancellationToken.Register(() =>
                {
                    timer.Dispose();
                    tcs.TrySetCanceled();
                });
            }

            // Start the timer and hand back the task...
            try { timer.Change(dueTime, Timeout.Infinite); }
            catch (ObjectDisposedException) { } // in case there's a race with cancellation; this is benign

            return tcs.Task;
        }
        #endregion

        #region TaskFactory with Action
        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime, Action action)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, action, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime, Action action, TaskCreationOptions creationOptions)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, action, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime, Action action, CancellationToken cancellationToken)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, action, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime, Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (dueTime < 0)
                throw new ArgumentOutOfRangeException("dueTime");
            if (action == null)
                throw new ArgumentNullException("action");
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            return factory
                .StartNewDelayed(dueTime, cancellationToken)
                .ContinueWith(t => action(), cancellationToken, TaskContinuationOptions.OnlyOnRanToCompletion, scheduler);
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime, Action<object> action, object state)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, action, state, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime, Action<object> action, object state, TaskCreationOptions creationOptions)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, action, state, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime, Action<object> action, object state, CancellationToken cancellationToken)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, action, state, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int dueTime, Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (dueTime < 0)
                throw new ArgumentOutOfRangeException("dueTime");
            if (action == null)
                throw new ArgumentNullException("action");
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            // Create the task that will be returned; workaround for no ContinueWith(..., state) overload.
            var result = new TaskCompletionSource<object>(state);

            // Delay a continuation to run the action
            factory
                .StartNewDelayed(dueTime, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsCanceled)
                    {
                        result.TrySetCanceled();
                        return;
                    }
                    try
                    {
                        action(state);
                        result.TrySetResult(null);
                    }
                    catch (Exception ex)
                    {
                        result.TrySetException(ex);
                    }
                }, scheduler);

            // Return the task
            return result.Task;
        }
        #endregion

        #region TaskFactory<TResult> with Func
        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory, int dueTime, Func<TResult> function)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, function, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory, int dueTime, Func<TResult> function, TaskCreationOptions creationOptions)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, function, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="cancellationToken">The CancellationToken to assign to the Task.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory, int dueTime, Func<TResult> function, CancellationToken cancellationToken)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, function, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="cancellationToken">The CancellationToken to assign to the Task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory, int dueTime, Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (dueTime < 0)
                throw new ArgumentOutOfRangeException("dueTime");
            if (function == null)
                throw new ArgumentNullException("function");
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            // Create the trigger and the timer to start it
            var tcs = new TaskCompletionSource<object>();
            var timer = new Timer(
              obj => ((TaskCompletionSource<object>)obj).SetResult(null),
              tcs, dueTime, Timeout.Infinite);

            // Return a task that executes the function when the trigger fires
            return tcs.Task.ContinueWith(t =>
            {
                timer.Dispose();
                return function();
            }, cancellationToken, ContinuationOptionsFromCreationOptions(creationOptions), scheduler);
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory, int dueTime, Func<object, TResult> function, object state)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, function, state, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="cancellationToken">The CancellationToken to assign to the Task.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory, int dueTime, Func<object, TResult> function, object state, CancellationToken cancellationToken)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, function, state, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory, int dueTime, Func<object, TResult> function, object state, TaskCreationOptions creationOptions)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return StartNewDelayed(factory, dueTime, function, state, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="dueTime">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="cancellationToken">The CancellationToken to assign to the Task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory, int dueTime, Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (dueTime < 0)
                throw new ArgumentOutOfRangeException("dueTime");
            if (function == null)
                throw new ArgumentNullException("function");
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            // Create the task that will be returned
            var result = new TaskCompletionSource<TResult>(state);
            Timer timer = null;

            // Create the task that will run the user's function
            var functionTask = new Task<TResult>(function, state, creationOptions);

            // When the function task completes, transfer the results to the returned task
            functionTask.ContinueWith(t =>
            {
                result.SetFromTask(t);
                timer.Dispose();
            }, cancellationToken, ContinuationOptionsFromCreationOptions(creationOptions) | TaskContinuationOptions.ExecuteSynchronously, scheduler);

            // Start the timer for the trigger
            timer = new Timer(obj => ((Task)obj).Start(scheduler),
                functionTask, dueTime, Timeout.Infinite);

            return result.Task;
        }
        #endregion

        /// <summary>Gets the TaskScheduler instance that should be used to schedule tasks.</summary>
        private static TaskScheduler GetTargetScheduler(this TaskFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return factory.Scheduler ?? TaskScheduler.Current;
        }

        /// <summary>Gets the TaskScheduler instance that should be used to schedule tasks.</summary>
        private static TaskScheduler GetTargetScheduler<TResult>(this TaskFactory<TResult> factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            return factory.Scheduler ?? TaskScheduler.Current;
        }

        /// <summary>Converts TaskCreationOptions into TaskContinuationOptions.</summary>
        private static TaskContinuationOptions ContinuationOptionsFromCreationOptions(TaskCreationOptions creationOptions)
        {
            return (TaskContinuationOptions)
                ((creationOptions & TaskCreationOptions.AttachedToParent) |
                 (creationOptions & TaskCreationOptions.PreferFairness) |
                 (creationOptions & TaskCreationOptions.LongRunning));
        }
    }
}

#endif
