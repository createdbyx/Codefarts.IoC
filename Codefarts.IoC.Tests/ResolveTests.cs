// <copyright file="ResolveTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Resolve")]
    public class ResolveTests
    {
        [TestMethod]
        public void Resolve_ValueType()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<int>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_String()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<string>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_Delegate()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<Container.Creator>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_ActionDelegate()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<Action>();
                Assert.IsNull(value);
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_GenericActionDelegate()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<Action<string>>();
                Assert.IsNull(value);
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_GenericFuncDelegate()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<Func<bool>>();
                Assert.IsNull(value);
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_AbstractClass()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<TestClassBase>();
                Assert.IsNull(value);
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_RegisteredTypesCount()
        {
            var container = new Container();

            Assert.AreEqual(0, container.RegisteredTypes.Count());

            container.Register<ITestInterface, TestClassDefaultCtor>();

            Assert.AreEqual(1, container.RegisteredTypes.Count());
            Assert.AreSame(typeof(ITestInterface), container.RegisteredTypes.FirstOrDefault());
        }

        [TestMethod]
        public void Resolve_RegisteredTypeWithImplementation_ReturnsInstanceOfCorrectType()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();

            var output = container.Resolve<ITestInterface>();

            Assert.IsInstanceOfType(output, typeof(TestClassDefaultCtor));
        }

        [TestMethod]
        public void Resolve_RegisteredTypeWithImplementation_ReturnsDifferentInstances()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();

            var output = container.Resolve<ITestInterface>();
            var output2 = container.Resolve<ITestInterface>();

            Assert.AreNotSame(output, output2);
        }

        [TestMethod]
        public void Resolve_TypeRegisteredWithDelegateFactoryStaticMethod_ResolvesCorrectlyUsingDelegateFactory()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => TestClassDefaultCtor.CreateNew(container));

            var output = container.Resolve<ITestInterface>() as TestClassDefaultCtor;

            Assert.AreEqual("Testing", output.Prop1);
        }

        [TestMethod]
        public void Resolve_TypeRegisteredWithDelegateFactoryLambda_ResolvesCorrectlyUsingDelegateFactory()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => new TestClassDefaultCtor() { Prop1 = "Testing" });

            var output = container.Resolve<ITestInterface>() as TestClassDefaultCtor;

            Assert.AreEqual("Testing", output.Prop1);
        }

        [TestMethod]
        public void Resolve_UnregisteredClassTypeWithDefaultCtor_ResolvesType()
        {
            var container = new Container();
            var output = container.Resolve<TestClassDefaultCtor>();

            Assert.IsInstanceOfType(output, typeof(TestClassDefaultCtor));
        }

        [TestMethod]
        public void Resolve_UnregisteredClassTypeWithDependencies_ResolvesType()
        {
            var container = new Container();

            var output = container.Resolve<TestClassWithDependency>();

            Assert.IsInstanceOfType(output, typeof(TestClassWithDependency));
        }

        [TestMethod]
        public void Resolve_UnregisteredInterface_ThrowsException()
        {
            var container = new Container();
            AssertHelper.ThrowsException<ContainerResolutionException>(() => container.Resolve<ITestInterface>());
        }

        [TestMethod]
        public void Resolve_UnregisteredClassWithUnregisteredInterfaceDependencies_ThrowsException()
        {
            var container = new Container();
            AssertHelper.ThrowsException<ContainerResolutionException>(
                () => container.Resolve<TestClassWithInterfaceDependency>());
        }

        [TestMethod]
        public void Resolve_RegisteredInterfaceWithUnregisteredInterfaceDependencies_ThrowsException()
        {
            var container = new Container();
            container.Register<ITestInterface2, TestClassWithInterfaceDependency>();

            AssertHelper.ThrowsException<ContainerResolutionException>(() => container.Resolve<ITestInterface2>());
        }

        [TestMethod]
        public void Resolve_FactoryRegisteredTypeThatThrows_ThrowsCorrectException()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => { throw new NotImplementedException(); });

            AssertHelper.ThrowsException<ContainerResolutionException>(() => container.Resolve<ITestInterface>());

            // Should have thrown by now
            // Assert.IsTrue(false);
        }

        [TestMethod]
        public void Resolve_ReturnsDifferentContainer()
        {
            var container = new Container();

            var result = container.Resolve<Container>();

            Assert.AreNotSame(result, container);
        }

        [TestMethod]
        public void Resolve_ClassWithTinyIoCDependency_Resolves()
        {
            var container = new Container();

            var result = container.Resolve<TestClassWithContainerDependency>();

            Assert.IsInstanceOfType(result, typeof(TestClassWithContainerDependency));
        }

        [TestMethod]
        public void Resolve_RegisteredInstance_SameInstance()
        {
            var container = new Container();
            var item = new DisposableTestClassWithInterface();
            container.Register<DisposableTestClassWithInterface>(() => item);

            var result = container.Resolve<DisposableTestClassWithInterface>();

            Assert.AreSame(item, result);
        }

        [TestMethod]
        public void Resolve_RegisteredInstanceWithInterface_SameInstance()
        {
            var container = new Container();
            var item = new DisposableTestClassWithInterface();
            container.Register<ITestInterface>(() => item);

            var result = container.Resolve<ITestInterface>();

            Assert.AreSame(item, result);
        }

        [TestMethod]
        public void Resolve_RegisteredGenericTypeImplementationOnlyCorrectGenericTypes_Resolves()
        {
            var container = new Container();
            // container.Register<GenericClassWithInterface<int, string>>();

            var result = container.Resolve<GenericClassWithInterface<int, string>>();

            Assert.IsInstanceOfType(result, typeof(GenericClassWithInterface<int, string>));
        }

        [TestMethod]
        public void Resolve_RegisteredGenericTypeWithInterfaceCorrectGenericTypes_Resolves()
        {
            var container = new Container();
            container.Register<ITestInterface, GenericClassWithInterface<int, string>>();

            var result = container.Resolve<ITestInterface>();

            Assert.IsInstanceOfType(result, typeof(GenericClassWithInterface<int, string>));
        }

        [TestMethod]
        public void Resolve_BoundGenericTypeWithoutRegistered_ResolvesWithDefaultOptions()
        {
            var container = new Container();

            var testing = container.Resolve<GenericClassWithInterface<int, string>>();

            Assert.IsInstanceOfType(testing, typeof(GenericClassWithInterface<int, string>));
        }

        [TestMethod]
        public void Resolve_BoundGenericTypeWithFailedDependenciesWithoutRegistered_ThrowsException()
        {
            var container = new Container();

            AssertHelper.ThrowsException<ContainerResolutionException>(
                () => container.Resolve<GenericClassWithParametersAndDependencies<int, string>>());

            // Assert.IsInstanceOfType(testing, typeof(GenericClassWithParametersAndDependencies<int, string>));
        }

        [TestMethod]
        public void Resolve_BoundGenericTypeWithDependenciesWithoutRegistered_ResolvesUsingCorrectCtor()
        {
            var container = new Container();
            container.Register<ITestInterface2, TestClass2>();

            var testing = container.Resolve<GenericClassWithParametersAndDependencies<int, string>>();

            Assert.IsInstanceOfType(testing, typeof(GenericClassWithParametersAndDependencies<int, string>));
        }

        [TestMethod]
        public void Resolve_ClassWithGenericFuncParameterInConstructor()
        {
            var container = new Container();

            var exceptionThrown = Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var result = container.Resolve<TestClassWithLazyFactory>();
                Assert.IsInstanceOfType(result, typeof(TestClassWithLazyFactory));
            });

            Assert.IsInstanceOfType(exceptionThrown, typeof(ContainerResolutionException));
        }

        [TestMethod]
        public void Resolve_MultiInstanceFactoryNoConstructorSpecified_UsesCorrectCtor()
        {
            var container = new Container();
            container.Register<TestClassDefaultCtor, TestClassDefaultCtor>();
            container.Register<TestClassMultiDepsMultiCtors, TestClassMultiDepsMultiCtors>();

            var result = container.Resolve<TestClassMultiDepsMultiCtors>();

            Assert.AreEqual(2, result.NumberOfDepsResolved);
        }

        [TestMethod]
        public void Resolve_DelegateFactoryThrowsException_ThrowsTinyIoCException()
        {
            var container = new Container();
            container.Register<TestClassConstructorFailure>(() => { throw new NotImplementedException(); });

            AssertHelper.ThrowsException<ContainerResolutionException>(
                () => container.Resolve<TestClassConstructorFailure>());

            // Should have thrown by now
            // Assert.IsTrue(false);
        }

        [TestMethod]
        public void Resolve_ConstructorThrowsException()
        {
            var container = new Container();
            //  container.Register<TestClassConstructorFailure>();

            AssertHelper.ThrowsException<ContainerResolutionException>(
                () => container.Resolve<TestClassConstructorFailure>());

            // Should have thrown by now
            // Assert.IsTrue(false);
        }

        [TestMethod]
        public void Resolve_TypeWithIEnumerableOfNonRegisteredTypeDependency()
        {
            var container = new Container();
            //  container.Register<TestClassEnumerableDependency>();

            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var result = container.Resolve<TestClassEnumerableDependency>();
                Assert.Fail("Should have thrown an ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_UnregisteredType_ResolvesWithIEnumerableParamWithNoItems()
        {
            var container = new Container();
            container.Register<IEnumerable<ITestInterface>>(() => Enumerable.Empty<ITestInterface>());

            var result = container.Resolve<TestClassEnumerableDependency>();
            Assert.IsNotNull(result, "Should have returned a reference to a TestClassEnumerableDependency class.");
        }

        [TestMethod]
        public void Resolve_UnregisteredType_FailsToResolveWithUnregisteredAbstractClassArgs()
        {
            var container = new Container();

            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var result = container.Resolve<TestClassWithAbstractDependency>();
                Assert.Fail("Should have thrown an ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_UnregisteredType_ResolvesWithRegisteredAbstractClassArgs()
        {
            var container = new Container();
            container.Register<TestClassBase, TestClassWithBaseClass>();

            var result = container.Resolve<TestClassWithAbstractDependency>();
            Assert.IsNotNull(result, "Should have returned a reference to a TestClassEnumerableDependency class.");
            Assert.IsNotNull(result.Prop1, "Unit test failure! Property should have been set in constructor.");
            Assert.IsInstanceOfType(result.Prop1, typeof(TestClassBase));
            Assert.IsInstanceOfType(result.Prop1, typeof(TestClassWithBaseClass));
        }
    }
}