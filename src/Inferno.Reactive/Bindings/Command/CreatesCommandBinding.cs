using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Inferno
{
    public class CreatesCommandBinding
    {
        private static MemoizingMRUCache<Type, ICreatesCommandBinding> bindCommandCache;

        private static MemoizingMRUCache<Type, ICreatesCommandBinding> bindCommandEventCache;

        internal static void Initialize(IEnumerable<ICreatesCommandBinding> factories)
        {
            if (factories == null) throw new ArgumentNullException(nameof(factories));

            bindCommandCache =
            new MemoizingMRUCache<Type, ICreatesCommandBinding>(
                (t, _) =>
                {
                    return factories
                        .Aggregate((score: 0, binding: (ICreatesCommandBinding)null), (acc, x) =>
                        {
                            int score = x.GetAffinityForObject(t, false);
                            return (score > acc.score) ? (score, x) : acc;
                        }).binding;
                }, RxApp.SmallCacheLimit);

            bindCommandEventCache =
            new MemoizingMRUCache<Type, ICreatesCommandBinding>(
                (t, _) =>
                {
                    return factories
                        .Aggregate((score: 0, binding: (ICreatesCommandBinding)null), (acc, x) =>
                        {
                            int score = x.GetAffinityForObject(t, true);
                            return (score > acc.score) ? (score, x) : acc;
                        }).binding;
                }, RxApp.SmallCacheLimit);
        }

        public static IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            var type = target.GetType();

            var binder = bindCommandCache.Get(type);

            if (binder == null)
            {
                throw new Exception($"Couldn't find a Command Binder for {type.FullName}");
            }

            var ret = binder.BindCommandToObject(command, target, commandParameter);
            if (ret == null)
            {
                throw new Exception($"Couldn't bind Command Binder for {type.FullName}");
            }

            return ret;
        }

        public static IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter, string eventName)
        {
            var type = target.GetType();
            var binder = bindCommandEventCache.Get(type);
            if (binder == null)
            {
                throw new Exception($"Couldn't find a Command Binder for {type.FullName} and event {eventName}");
            }

            var eventArgsType = Reflection.GetEventArgsTypeForEvent(type, eventName);
            var mi = binder.GetType().GetTypeInfo().DeclaredMethods.First(x => x.Name == "BindCommandToObject" && x.IsGenericMethod);
            mi = mi.MakeGenericMethod(new[] { eventArgsType });

            var ret = (IDisposable)mi.Invoke(binder, new[] { command, target, commandParameter, eventName });
            if (ret == null)
            {
                throw new Exception($"Couldn't bind Command Binder for {type.FullName} and event {eventName}");
            }

            return ret;
        }
    }
}
