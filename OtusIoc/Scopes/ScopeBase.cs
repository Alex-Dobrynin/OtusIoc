namespace OtusIoc.Scopes
{
    internal abstract class ScopeBase : IScope
    {
        protected Dictionary<string, object> Store = new();

        public abstract string Name { get; }

        public virtual void Dispose()
        {

        }

        internal virtual object Resolve(string key)
        {
            if (Store.ContainsKey(key)) return Store[key];
            throw new InvalidOperationException($"There is no such object registered for given {key} key");
        }

        internal virtual void Register(string key, object value)
        {
            Store[key] = value;
        }
    }
}
