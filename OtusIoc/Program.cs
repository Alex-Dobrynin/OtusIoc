using OtusIoc.Commands;
using OtusIoc.Constants;
using OtusIoc.Ioc;
using OtusIoc.Scopes;

Console.WriteLine("Hello, World!");

var locator = ServiceLocator.Instance;

locator.Resolve<ICommand>(StringConstants.IocRegister, "ooo", () => 2).Execute();

var d = locator.Resolve<int>("ooo");

var s = locator.Resolve<IScope>(StringConstants.CurrentScope);

using (var newSDisp = locator.Resolve<IScope>(StringConstants.CreateNewScope, s))
{
    var newS = locator.Resolve<IScope>(StringConstants.CurrentScope);
    locator.Resolve<ICommand>(StringConstants.IocRegister, "aaa", () => 3).Execute();
    locator.Resolve<ICommand>(StringConstants.IocRegister, "bbb", () => 5).Execute();
    var z = locator.Resolve<int>("bbb");
    d = locator.Resolve<int>("aaa");

    using (locator.Resolve<IScope>(StringConstants.CreateNewScope, newSDisp))
    {
        var newSS = locator.Resolve<IScope>(StringConstants.CurrentScope);
    }
}

var thread = new Thread(() =>
{
    locator.Resolve<ICommand>(StringConstants.IocRegister, "aaa", () => 4).Execute();
    var d = locator.Resolve<int>("aaa");

});
thread.Start();
thread.Join();

s = locator.Resolve<IScope>(StringConstants.CurrentScope);

var c = 0;