// <copyright file="ResolveTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Codefarts.IoC.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("Resolve")]
    public class ResolveTests
    {
        [TestMethod]
        public void ResolveProviderThrowsException()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => throw new Exception("Unit Test Exception!"));
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<ITestInterface>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void ResolveNestedProviderThrowsException()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => throw new Exception("Unit Test Exception!"));
            container.Register<TestClassMultiInterfaceDepsMultiCtors>(
                () => new TestClassMultiInterfaceDepsMultiCtors(container.Resolve<ITestInterface>(), container.Resolve<ITestInterface>()));
            container.Register<ITestMultiDeps, TestClassMultiInterfaceDepsMultiCtors>();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<ITestMultiDeps>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void ResolveIEnumerableDependencyWithoutRegistration()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<TestClassEnumerableDependency>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void ResolveIEnumerableDependencyWithRegistration()
        {
            var container = new Container();
            container.Register<IEnumerable<ITestInterface>, List<ITestInterface>>();

            Assert.ThrowsException<ExceededMaxInstantiationDepthException>(() =>
            {
                var value = container.Resolve<TestClassEnumerableDependency>();
                Assert.Fail($"Should have thrown {nameof(ExceededMaxInstantiationDepthException)} or stack overflowed.");
            });
        }

        [TestMethod]
        public void ResolveIEnumerableDependencyWithRegistration_MultiThreaded()
        {
            var container = new Container();
            container.Register<IEnumerable<ITestInterface>, List<ITestInterface>>();

            var tasks = Enumerable.Range(0, 100).Select(x => new Task(() =>
            {
                Assert.ThrowsException<ExceededMaxInstantiationDepthException>(() =>
                {
                    var value = container.Resolve<TestClassEnumerableDependency>();
                    Assert.Fail($"Should have thrown {nameof(ExceededMaxInstantiationDepthException)} or stack overflowed.");
                });
            })).ToArray();

            Array.ForEach(tasks, t => t.Start());
            Task.WaitAll(tasks);
        }

        [TestMethod]
        public void ResolveIEnumerableDependencyWithRegistrationProvider()
        {
            var container = new Container();
            container.Register<IEnumerable<ITestInterface>>(() => new List<ITestInterface>());

            var value = container.Resolve<TestClassEnumerableDependency>();
            Assert.IsNotNull(value);
            Assert.IsNotNull(value.Enumerable);
            Assert.AreEqual(0, value.EnumerableCount);
        }

        [TestMethod]
        public void ResolveGenericIEnumerable()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<IEnumerable<ITestInterface>>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void ResolveIEnumerable()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<IEnumerable>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void ResolveValueType()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<int>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void ResolveString()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<string>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void ResolveDelegate()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<Container.Creator>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void ActionDelegate()
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
        public void GenericActionDelegate()
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
        public void GenericFuncDelegate()
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
        public void AbstractClass()
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
        public void RegisteredTypesCount()
        {
            var container = new Container();

            Assert.AreEqual(0, container.RegisteredTypes.Count());

            container.Register<ITestInterface, TestClassDefaultCtor>();

            Assert.AreEqual(1, container.RegisteredTypes.Count());
            Assert.AreSame(typeof(ITestInterface), container.RegisteredTypes.FirstOrDefault());
        }

        [TestMethod]
        public void RegisteredTypeWithImplementation_ReturnsInstanceOfCorrectType()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();

            var output = container.Resolve<ITestInterface>();

            Assert.IsInstanceOfType(output, typeof(TestClassDefaultCtor));
        }

        [TestMethod]
        public void RegisteredTypeWithImplementation_ReturnsDifferentInstances()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();

            var output = container.Resolve<ITestInterface>();
            var output2 = container.Resolve<ITestInterface>();

            Assert.AreNotSame(output, output2);
        }

        [TestMethod]
        public void TypeRegisteredWithDelegateFactoryStaticMethod_ResolvesCorrectlyUsingDelegateFactory()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => TestClassDefaultCtor.CreateNew(container));

            var output = container.Resolve<ITestInterface>() as TestClassDefaultCtor;

            Assert.AreEqual("Testing", output.Prop1);
        }

        [TestMethod]
        public void TypeRegisteredWithDelegateFactoryLambda_ResolvesCorrectlyUsingDelegateFactory()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => new TestClassDefaultCtor() { Prop1 = "Testing" });

            var output = container.Resolve<ITestInterface>() as TestClassDefaultCtor;

            Assert.AreEqual("Testing", output.Prop1);
        }

        [TestMethod]
        public void UnregisteredClassTypeWithDefaultCtor_ResolvesType()
        {
            var container = new Container();
            var output = container.Resolve<TestClassDefaultCtor>();

            Assert.IsInstanceOfType(output, typeof(TestClassDefaultCtor));
        }

        [TestMethod]
        public void UnregisteredClassTypeWithDependencies_ResolvesType()
        {
            var container = new Container();

            var output = container.Resolve<TestClassWithDependency>();

            Assert.IsInstanceOfType(output, typeof(TestClassWithDependency));
        }

        [TestMethod]
        public void UnregisteredInterface_ThrowsException()
        {
            var container = new Container();
            AssertHelper.ThrowsException<ContainerResolutionException>(() => container.Resolve<ITestInterface>());
        }

        [TestMethod]
        public void UnregisteredClassWithUnregisteredInterfaceDependencies_ThrowsException()
        {
            var container = new Container();
            AssertHelper.ThrowsException<ContainerResolutionException>(
                () => container.Resolve<TestClassWithInterfaceDependency>());
        }

        [TestMethod]
        public void RegisteredInterfaceWithUnregisteredInterfaceDependencies_ThrowsException()
        {
            var container = new Container();
            container.Register<ITestInterface2, TestClassWithInterfaceDependency>();

            AssertHelper.ThrowsException<ContainerResolutionException>(() => container.Resolve<ITestInterface2>());
        }

        [TestMethod]
        public void FactoryRegisteredTypeThatThrows_ThrowsCorrectException()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => { throw new NotImplementedException(); });

            AssertHelper.ThrowsException<ContainerResolutionException>(() => container.Resolve<ITestInterface>());

            // Should have thrown by now
            // Assert.IsTrue(false);
        }

        [TestMethod]
        public void ReturnsDifferentContainer()
        {
            var container = new Container();

            var result = container.Resolve<Container>();

            Assert.AreNotSame(result, container);
        }

        [TestMethod]
        public void ClassWithTinyIoCDependency_Resolves()
        {
            var container = new Container();

            var result = container.Resolve<TestClassWithContainerDependency>();

            Assert.IsInstanceOfType(result, typeof(TestClassWithContainerDependency));
        }

        [TestMethod]
        public void RegisteredInstance_SameInstance()
        {
            var container = new Container();
            var item = new DisposableTestClassWithInterface();
            container.Register<DisposableTestClassWithInterface>(() => item);

            var result = container.Resolve<DisposableTestClassWithInterface>();

            Assert.AreSame(item, result);
        }

        [TestMethod]
        public void RegisteredInstanceWithInterface_SameInstance()
        {
            var container = new Container();
            var item = new DisposableTestClassWithInterface();
            container.Register<ITestInterface>(() => item);

            var result = container.Resolve<ITestInterface>();

            Assert.AreSame(item, result);
        }

        [TestMethod]
        public void RegisteredGenericTypeImplementationOnlyCorrectGenericTypes_Resolves()
        {
            var container = new Container();
            // container.Register<GenericClassWithInterface<int, string>>();

            var result = container.Resolve<GenericClassWithInterface<int, string>>();

            Assert.IsInstanceOfType(result, typeof(GenericClassWithInterface<int, string>));
        }

        [TestMethod]
        public void RegisteredGenericTypeWithInterfaceCorrectGenericTypes_Resolves()
        {
            var container = new Container();
            container.Register<ITestInterface, GenericClassWithInterface<int, string>>();

            var result = container.Resolve<ITestInterface>();

            Assert.IsInstanceOfType(result, typeof(GenericClassWithInterface<int, string>));
        }

        [TestMethod]
        public void BoundGenericTypeWithoutRegistered_ResolvesWithDefaultOptions()
        {
            var container = new Container();

            var testing = container.Resolve<GenericClassWithInterface<int, string>>();

            Assert.IsInstanceOfType(testing, typeof(GenericClassWithInterface<int, string>));
        }

        [TestMethod]
        public void BoundGenericTypeWithFailedDependenciesWithoutRegistered_ThrowsException()
        {
            var container = new Container();

            AssertHelper.ThrowsException<ContainerResolutionException>(
                () => container.Resolve<GenericClassWithParametersAndDependencies<int, string>>());

            // Assert.IsInstanceOfType(testing, typeof(GenericClassWithParametersAndDependencies<int, string>));
        }

        [TestMethod]
        public void BoundGenericTypeWithDependenciesWithoutRegistered_ResolvesUsingCorrectCtor()
        {
            var container = new Container();
            container.Register<ITestInterface2, TestClass2>();

            var testing = container.Resolve<GenericClassWithParametersAndDependencies<int, string>>();

            Assert.IsInstanceOfType(testing, typeof(GenericClassWithParametersAndDependencies<int, string>));
            Assert.IsNotNull(testing.Dependency);
            Assert.IsInstanceOfType(testing.Dependency, typeof(TestClass2));
        }

        [TestMethod]
        public void ClassWithGenericFuncParameterInConstructor()
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
        public void MultiInstanceFactoryNoConstructorSpecified_UsesCorrectCtor()
        {
            var container = new Container();
            // container.Register<TestClassDefaultCtor, TestClassDefaultCtor>();
            // container.Register<TestClassMultiDepsMultiCtors, TestClassMultiDepsMultiCtors>();

            var result = container.Resolve<TestClassMultiDepsMultiCtors>();

            Assert.AreEqual(2, result.NumberOfDepsResolved);
        }

        [TestMethod]
        public void DelegateFactoryThrowsException_ThrowsTinyIoCException()
        {
            var container = new Container();
            container.Register<TestClassConstructorFailure>(() => { throw new NotImplementedException(); });

            AssertHelper.ThrowsException<ContainerResolutionException>(
                () => container.Resolve<TestClassConstructorFailure>());

            // Should have thrown by now
            // Assert.IsTrue(false);
        }

        [TestMethod]
        public void ConstructorThrowsException()
        {
            var container = new Container();
            //  container.Register<TestClassConstructorFailure>();

            AssertHelper.ThrowsException<ContainerResolutionException>(
                () => container.Resolve<TestClassConstructorFailure>());

            // Should have thrown by now
            // Assert.IsTrue(false);
        }

        [TestMethod]
        public void TypeWithIEnumerableOfNonRegisteredTypeDependency()
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
        public void UnregisteredType_ResolvesWithIEnumerableParamWithNoItems()
        {
            var container = new Container();
            container.Register<IEnumerable<ITestInterface>>(() => Enumerable.Empty<ITestInterface>());

            var result = container.Resolve<TestClassEnumerableDependency>();
            Assert.IsNotNull(result, "Should have returned a reference to a TestClassEnumerableDependency class.");
        }

        [TestMethod]
        public void UnregisteredType_FailsToResolveWithUnregisteredAbstractClassArgs()
        {
            var container = new Container();

            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var result = container.Resolve<TestClassWithAbstractDependency>();
                Assert.Fail("Should have thrown an ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void UnregisteredType_ResolvesWithRegisteredAbstractClassArgs()
        {
            var container = new Container();
            container.Register<TestClassBase, TestClassWithBaseClass>();

            var result = container.Resolve<TestClassWithAbstractDependency>();
            Assert.IsNotNull(result, "Should have returned a reference to a TestClassEnumerableDependency class.");
            Assert.IsNotNull(result.Prop1, "Unit test failure! Property should have been set in constructor.");
            Assert.IsInstanceOfType(result.Prop1, typeof(TestClassBase));
            Assert.IsInstanceOfType(result.Prop1, typeof(TestClassWithBaseClass));
        }

        [TestMethod]
        public void ResolveMembersSimple()
        {
            var container = new Container();

            var someObject = new SimpleResolveMemberClass();

            container.ResolveMembers(someObject);
            Assert.IsNotNull(someObject.TestClassProperty, "Should have been assigned a value.");
        }

        [TestMethod]
        public void ResolveMembersNulArg()
        {
            var container = new Container();

            Assert.ThrowsException<ArgumentNullException>(() => { container.ResolveMembers(null); });
        }

        [TestMethod]
        public void ResolveMembersFieldAlreadySet()
        {
            var container = new Container();
            var someValue = new SimpleResolveMemberClass();
            var fieldValue = container.Resolve<TestClassDefaultCtor>();
            someValue.TestClassField = fieldValue;
            container.ResolveMembers(someValue);

            Assert.AreSame(someValue.TestClassField, fieldValue);
        }

        [TestMethod]
        public void ResolveMembersFieldInstanciationWillThrowException()
        {
            var container = new Container();
            var someValue = new SimpleResolveMemberClassWithUnresolvableField();
            Assert.ThrowsException<ContainerResolutionException>(() => { container.ResolveMembers(someValue); });
        }

        [TestMethod]
        public void ResolveMembersFieldInstanciationWillThrowException_Supressed()
        {
            var container = new Container();
            var someValue = new SimpleResolveMemberClassWithUnresolvableField();
            container.ResolveMembers(someValue, true);

            Assert.IsNotNull(someValue.TestClassProperty);
            Assert.IsNull(someValue.TestClassField);
        }

        [TestMethod]
        public void ResolveMembersFieldInstanciationSupressedExceptions()
        {
            var container = new Container();
            var someValue = new SimpleResolveMemberClass();
            container.ResolveMembers(someValue, true);

            Assert.IsNotNull(someValue.TestClassProperty);
            Assert.IsNotNull(someValue.TestClassField);
        }

        [TestMethod]
        public void ResolveWithDefaultFallbackSuccess()
        {
            var container = new Container();

            var defaultValue = new SimpleResolveMemberClass();
            defaultValue.TestClassField = new TestClassDefaultCtor();
            defaultValue.TestClassProperty = new TestClassDefaultCtor();
            defaultValue.TestClassProperty.Prop1 = "Prop";
            defaultValue.TestClassField.Prop1 = "Field";
            var someValue = container.Resolve<SimpleResolveMemberClass>(defaultValue);

            Assert.IsNotNull(someValue);
            Assert.IsNull(someValue.TestClassProperty);
            Assert.IsNull(someValue.TestClassField);
        }

        [TestMethod]
        public void ResolveWithDefaultFallbackFailure()
        {
            var container = new Container();

            var defaultValue = new SimpleClassCtorThrowsExeption(false);
            defaultValue.TestClassField = new TestClassWithInterfaceDependency(new TestClassDefaultCtor(), 123, "Prop");
            defaultValue.TestClassProperty = new TestClassDefaultCtor();
            var someValue = container.Resolve<SimpleClassCtorThrowsExeption>(defaultValue);

            Assert.AreSame(defaultValue, someValue);
            Assert.IsNotNull(someValue);
            Assert.IsNotNull(someValue.TestClassField);
            Assert.IsNotNull(someValue.TestClassProperty);
            Assert.AreEqual("Prop", someValue.TestClassField.Param2);
            Assert.AreEqual(123, someValue.TestClassField.Param1);
        }
    }
}