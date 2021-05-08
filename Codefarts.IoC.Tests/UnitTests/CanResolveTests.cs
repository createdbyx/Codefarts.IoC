// <copyright file="CanResolveTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("CanResolve")]
    public class CanResolveTests
    {
        [TestMethod]
        public void CanResolveType_WithNoRegistration_Returnsfalse()
        {
            var container = new Container();

            var result = container.CanResolve<ITestInterface>();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanResolveType_UnregisteredTypeDefaultCtor_ReturnsFalse()
        {
            var container = new Container();
            var result = container.CanResolve<TestClassDefaultCtor>();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanResolveType_UnregisteredInterface_ReturnsFalse()
        {
            var container = new Container();
            var result = container.CanResolve<ITestInterface>();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanResolveType_RegisteredInterface_ReturnsTrue()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();

            var result = container.CanResolve<ITestInterface>();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanResolveType_FactoryRegisteredType_ReturnsTrue()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => TestClassDefaultCtor.CreateNew(container));

            var result = container.CanResolve<ITestInterface>();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanResolveType_FactoryRegisteredTypeThatThrows_ReturnsTrue()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => { throw new NotImplementedException(); });

            var result = container.CanResolve<ITestInterface>();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanResolve_BoundGenericTypeWithoutRegistered_ReturnsFalse()
        {
            var container = new Container();

            var testing = container.CanResolve<GenericClassWithInterface<int, string>>();

            Assert.IsFalse(testing);
        }

        [TestMethod]
        public void CanResolve_ClassNamedWithLazyFactoryDependency_ReturnsFalse()
        {
            var container = new Container();

            var result = container.CanResolve<TestClassWithNamedLazyFactory>();

            Assert.IsFalse(result);
        }
    }
}