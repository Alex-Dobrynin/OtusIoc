using OtusIoc.Constants;
using OtusIoc.Ioc;
using OtusIoc.Scopes;

namespace OtusIoc.Commands
{
    internal class RegisterCommand : ICommand
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly string _key;
        private readonly object _value;

        public RegisterCommand(IServiceLocator serviceLocator, string key, object value)
        {
            _serviceLocator = serviceLocator;
            _key = key;
            _value = value;
        }

        public void Execute()
        {
            _serviceLocator.Resolve<ScopeBase>(StringConstants.CurrentScope).Register(_key, _value);
        }
    }
}
