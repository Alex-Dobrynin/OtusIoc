using OtusIoc.Commands;
using OtusIoc.Constants;
using OtusIoc.Ioc;

namespace OtusIoc.Scopes
{
    internal class ChildScope : ScopeBase
    {
        private readonly IServiceLocator _locator;

        private readonly ScopeBase _parentScope;
        private readonly IScope _oldScope;

        public ChildScope(IServiceLocator locator, ScopeBase parentScope)
        {
            _locator = locator;
            _parentScope = parentScope;

            _oldScope = _locator.Resolve<IScope>(StringConstants.CurrentScope);

            _locator.Resolve<ICommand>(StringConstants.SetCurrentScope, this).Execute();
        }

        public override string Name { get; } = Guid.NewGuid().ToString();

        public override void Dispose()
        {
            _locator.Resolve<ICommand>(StringConstants.SetCurrentScope, _oldScope).Execute();
        }

        internal override object Resolve(string key)
        {
            if (Store.ContainsKey(key)) return Store[key];
            return _parentScope.Resolve(key);
        }
    }
}
