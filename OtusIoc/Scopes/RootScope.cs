using OtusIoc.Commands;
using OtusIoc.Constants;
using OtusIoc.Ioc;

namespace OtusIoc.Scopes
{
    internal class RootScope : ScopeBase
    {
        public RootScope(IScopedLocator locator)
        {
            Store.Add(StringConstants.IocRegister, (string key, object value) => new RegisterCommand(locator, key, value));
            Store.Add(StringConstants.CurrentScope, () => locator.GetCurrentScope());
            Store.Add(StringConstants.SetCurrentScope, (ScopeBase scope) => new SetScopeCommand(locator, scope));
            Store.Add(StringConstants.CreateNewScope, (ScopeBase scope) => new ChildScope(locator, scope));
        }

        public override string Name => "Root";

        public override void Dispose()
        {

        }
    }
}
