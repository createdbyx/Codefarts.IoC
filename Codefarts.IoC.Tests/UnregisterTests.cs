// <copyright file="UnregisterTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Unregister")]
    public class UnregisterTests
    {
        [TestMethod]
        public void Unregister_RegisteredImplementation_CanUnregister()
        {
            var container = new Container();
            container.Register<TestClassDefaultCtor, TestClassDefaultCtor>();

            var unregistered = container.Unregister(typeof(TestClassDefaultCtor));
            var resolved = container.CanResolve<TestClassDefaultCtor>();

            Assert.IsTrue(unregistered);
            Assert.IsFalse(resolved);
        }

        [TestMethod]
        public void Unregister_NotRegisteredImplementation_CannotUnregister()
        {
            var container = new Container();

            var unregistered = container.Unregister(typeof(TestClassDefaultCtor));

            Assert.IsFalse(unregistered);
        }

        [TestMethod]
        public void Unregister_RegisteredInterface_CanUnregister()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();

            var unregistered = container.Unregister(typeof(ITestInterface));
            var resolved = container.CanResolve<ITestInterface>();

            Assert.IsTrue(unregistered);
            Assert.IsFalse(resolved);
        }

        [TestMethod]
        public void Unregister_NotRegisteredInterface_CannotUnregister()
        {
            var container = new Container();

            var unregistered = container.Unregister(typeof(ITestInterface));

            Assert.IsFalse(unregistered);
        }

        [TestMethod]
        public void Unregister_T_RegisteredImplementation_CanUnregister()
        {
            var container = new Container();
            container.Register<TestClassDefaultCtor, TestClassDefaultCtor>();

            var unregistered = container.Unregister<TestClassDefaultCtor>();
            var resolved = container.CanResolve<TestClassDefaultCtor>();

            Assert.IsTrue(unregistered);
            Assert.IsFalse(resolved);
        }

        [TestMethod]
        public void Unregister_T_NotRegisteredImplementation_CannotUnregister()
        {
            var container = new Container();

            var unregistered = container.Unregister<TestClassDefaultCtor>();

            Assert.IsFalse(unregistered);
        }

        [TestMethod]
        public void Unregister_T_RegisteredInterface_CanUnregister()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();

            var unregistered = container.Unregister<ITestInterface>();
            var resolved = container.CanResolve<ITestInterface>();

            Assert.IsTrue(unregistered);
            Assert.IsFalse(resolved);
        }

        [TestMethod]
        public void Unregister_T_NotRegisteredInterface_CannotUnregister()
        {
            var container = new Container();

            var unregistered = container.Unregister<ITestInterface>();

            Assert.IsFalse(unregistered);
        }

        [TestMethod]
        public void UnRegister()
        {
            var container = new Container();
            container.Register<IRepository>(() => new MockRepository());
            Assert.IsTrue(container.CanResolve<IRepository>());
            var value = container.Resolve<IRepository>();
            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(IRepository));
            Assert.IsInstanceOfType(value, typeof(MockRepository));

            container.Unregister<IRepository>();
            try
            {
                value = null;
                value = container.Resolve<IRepository>();
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ContainerResolutionException));
                Assert.IsNull(value);
            }
        }
    }
}