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

        // Scopes will grow with new threads
        // and there is no way to clean not used scopes
        // because we don't know when thread is done
        // and this will cause a memory leak
        public ThreadLocal<IScope> Scopes { get; } = new ThreadLocal<IScope>();

        private ScopeBase InitScope()
        {
            ScopeBase scope;
            if (Scopes.IsValueCreated) scope = Scopes.Value as ScopeBase;
            else
            {
                scope = new RootScope(this);
                Scopes.Value = scope;
            }

            return scope;
        }

        public T Resolve<T>(string key, params object[] args)
        {
            var scope = InitScope();

            var value = scope.Resolve(key);

            return (T)this.HandleReslove(value, args);
        }

        private object HandleReslove(object value, params object[] args)
        {
            return value switch
            {
                Delegate buildFunc => DelegateHandle(buildFunc, args),
                Type type => TypeHandle(type, args),
                _ => value
            };
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
