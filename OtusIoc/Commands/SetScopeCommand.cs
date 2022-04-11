using OtusIoc.Ioc;
using OtusIoc.Scopes;

namespace OtusIoc.Commands
{
    internal class SetScopeCommand : ICommand
    {
        private readonly IScopedLocator _scopedLocator;
        private readonly ScopeBase _scope;

        public SetScopeCommand(IScopedLocator scopedLocator, ScopeBase scope)
        {
            _scopedLocator = scopedLocator;
            _scope = scope;
        }

        public void Execute()
        {
            _scopedLocator.SetCurrentScope(_scope);
        }
    }
}
