using OtusIoc.Scopes;

namespace OtusIoc.Ioc
{
    public interface IServiceLocator
    {
        T Resolve<T>(string key, params object[] args);
    }

    public interface IScopedLocator : IServiceLocator
    {
        void SetCurrentScope(IScope scope);
        IScope GetCurrentScope();
    }

    public sealed class ServiceLocator : IScopedLocator
    {
        #region Thread-safe Singleton
        private static readonly ServiceLocator _instance = new ServiceLocator();

        static ServiceLocator()
        {
        }

        private ServiceLocator()
        {
            Scopes.Value = new RootScope(this);

            _handlers.Add(typeof(Type), TypeHandle);
            _handlers.Add(typeof(Delegate), DelegateHandle);
            _handlers.Add(typeof(object), ObjectHandle);
        }

        public static IServiceLocator Instance => _instance;
        #endregion

        public void SetCurrentScope(IScope scope)
        {
            if (scope is ScopeBase)
            {
                Scopes.Value = scope;
            }
        }

        public IScope GetCurrentScope() => Scopes.Value;

        public ThreadLocal<IScope> Scopes { get; } = new ThreadLocal<IScope>();

        private ScopeBase CheckScope()
        {
            return (Scopes.Value as ScopeBase) ?? throw new NullReferenceException("Scope for this thread is not initialized");
        }

        public T Resolve<T>(string key, params object[] args)
        {
            var scope = CheckScope();

            var value = scope.Resolve(key);

            return (T)this.HandleReslove(value, args);
        }

        private object HandleReslove(object value, params object[] args)
        {
            Delegate handler = null;

            foreach (var kvpHandler in _handlers)
            {
                if (kvpHandler.Key.IsAssignableFrom(value.GetType()))
                {
                    handler = kvpHandler.Value;
                    break;
                }
            }

            return handler?.DynamicInvoke(value, args) ?? throw new ArgumentException();
        }

        private Dictionary<Type, Delegate> _handlers = new();

        private object ObjectHandle(object value, params object[] args)
        {
            return value;
        }

        private object TypeHandle(Type type, params object[] args)
        {
            return Activator.CreateInstance(type, args: args);
        }

        private object DelegateHandle(Delegate buildFunc, params object[] args)
        {
            return buildFunc?.DynamicInvoke(args);
        }
    }
}
