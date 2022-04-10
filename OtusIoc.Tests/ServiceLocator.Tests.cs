using System;
using System.Collections.Generic;
using System.Threading;

using FluentAssertions;

using Moq;

using OtusIoc.Commands;
using OtusIoc.Constants;
using OtusIoc.Ioc;
using OtusIoc.Scopes;

using Xunit;

namespace OtusIoc.Tests
{
    public class ServiceLocatorTests
    {
        public ServiceLocatorTests()
        {

        }

        [Fact]
        public void ServiceLocator_ShouldResolve_RegisteredDelegate()
        {
            var locator = ServiceLocator.Instance;
            locator.Resolve<ICommand>(StringConstants.IocRegister, "delegate", () => 2).Execute();

            var num = locator.Resolve<int>("delegate");

            num.Should().Be(2);
        }

        [Fact]
        public void ServiceLocator_ShouldResolve_RegisteredType()
        {
            var locator = ServiceLocator.Instance;
            locator.Resolve<ICommand>(StringConstants.IocRegister, "type", typeof(int)).Execute();

            var num = locator.Resolve<int>("type");

            num.Should().Be(default);
        }

        [Fact]
        public void ServiceLocator_ShouldResolve_RegisteredType_WithArguments()
        {
            var locator = ServiceLocator.Instance;
            locator.Resolve<ICommand>(StringConstants.IocRegister, "type_with_arguments", typeof(List<int>)).Execute();

            var list = locator.Resolve<List<int>>("type_with_arguments", 2);

            list.Capacity.Should().Be(2);
        }

        [Fact]
        public void ServiceLocator_ShouldResolve_RegisteredObject()
        {
            var dummyCommand = Mock.Of<ICommand>();
            var locator = ServiceLocator.Instance;
            locator.Resolve<ICommand>(StringConstants.IocRegister, "object", dummyCommand).Execute();

            var resolvedCommand = locator.Resolve<ICommand>("object");

            resolvedCommand.Should().NotBeNull();
        }

        [Fact]
        public void ServiceLocator_ShouldResolve_CurrentScope()
        {
            var locator = ServiceLocator.Instance;

            var scope = locator.Resolve<IScope>(StringConstants.CurrentScope);

            scope.Should().NotBeNull();
        }

        [Fact]
        public void ServiceLocator_ShouldResolveNewScope_BasedOnAnother()
        {
            var locator = ServiceLocator.Instance;

            var baseScope = locator.Resolve<IScope>(StringConstants.CurrentScope);

            using (var dispScope = locator.Resolve<IScope>(StringConstants.CreateNewScope, baseScope))
            {
                var newScope = locator.Resolve<IScope>(StringConstants.CurrentScope);
                newScope.Should().Be(dispScope);

                using (var anotherDispScope = locator.Resolve<IScope>(StringConstants.CreateNewScope, dispScope))
                {
                    newScope = locator.Resolve<IScope>(StringConstants.CurrentScope);
                    newScope.Should().Be(anotherDispScope);
                }
            }
        }

        [Fact]
        public void ServiceLocator_ShouldSwitchScopes_WhenResolveNew_AndDisposeOld()
        {
            var locator = ServiceLocator.Instance;

            var baseScope = locator.Resolve<IScope>(StringConstants.CurrentScope);

            using (var dispScope = locator.Resolve<IScope>(StringConstants.CreateNewScope, baseScope))
            {
                dispScope.Should().NotBe(baseScope);

                var newScope = locator.Resolve<IScope>(StringConstants.CurrentScope);
                newScope.Should().Be(dispScope);

                using (var anotherDispScope = locator.Resolve<IScope>(StringConstants.CreateNewScope, dispScope))
                {
                    anotherDispScope.Should().NotBe(dispScope);

                    var anotherNewScope = locator.Resolve<IScope>(StringConstants.CurrentScope);
                    anotherNewScope.Should().Be(anotherDispScope);
                }

                newScope = locator.Resolve<IScope>(StringConstants.CurrentScope);
                newScope.Should().Be(dispScope);
            }

            baseScope = locator.Resolve<IScope>(StringConstants.CurrentScope);
            baseScope.Should().NotBeNull();
        }

        [Fact]
        public void ServiceLocator_ShouldUseDifferentScopes_ForDifferentThreads_WhenMultiThreading()
        {
            var locator = ServiceLocator.Instance;
            IScope anotherThreadScope = null;
            Exception exception = null;

            var mainThreadScope = locator.Resolve<IScope>(StringConstants.CurrentScope);

            var thread = new Thread(() =>
            {
                try
                {
                    anotherThreadScope = locator.Resolve<IScope>(StringConstants.CurrentScope);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            thread.Start();
            thread.Join();

            exception.Should().BeNull();
            anotherThreadScope.Should().NotBeNull().And.NotBe(mainThreadScope);
        }
    }
}