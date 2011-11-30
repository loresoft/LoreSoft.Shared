using System;
using System.Reflection;

namespace LoreSoft.Shared.Messaging
{
    internal class WeakAction
    {
        private readonly Type _parameterType;
        private readonly Type _delegateType;
        private readonly MethodInfo _method;
        private readonly WeakReference _targetReference;

        internal WeakAction(object target, MethodInfo method, Type parameterType)
        {
            _targetReference = target == null
              ? null
              : new WeakReference(target);

            _method = method;
            _parameterType = parameterType;

            _delegateType = parameterType == null
              ? typeof(Action)
              : typeof(Action<>).MakeGenericType(parameterType);
        }

        internal Type ParameterType
        {
            get { return _parameterType; }
        }

        internal WeakReference TargetReference
        {
            get { return _targetReference; }
        }

        internal MethodInfo Method
        {
            get { return _method; }
        }

        internal bool IsAlive
        {
            get { return _targetReference != null && _targetReference.IsAlive; }
        }

        internal Delegate CreateDelegate()
        {
            object target = null;

            if (_targetReference != null)
            {
                if (!_targetReference.IsAlive)
                    return null;

                target = _targetReference.Target;
                if (target == null)
                    return null;
            }

            try
            {
                return target == null
                  ? Delegate.CreateDelegate(_delegateType, _method)
                  : Delegate.CreateDelegate(_delegateType, target, _method);
            }
            catch (MethodAccessException ex)
            {
#if SILVERLIGHT
                throw new InvalidOperationException("The subscribed delegate must specify an accessible method in Silverlight.", ex);
#else
                throw;
#endif
            }
        }

        internal bool IsTarget(object target)
        {
            if (_targetReference == null)
                return false;

            return ReferenceEquals(target, _targetReference.Target);
        }
    }
}