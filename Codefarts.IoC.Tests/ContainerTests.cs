using System.Linq;
using System.Runtime.InteropServices;

namespace Codefarts.IoC.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Codefarts.IoC;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public partial class ContainerTests
    {
        [TestMethod]
        public void Default_Get_ReturnsInstanceOfContainer()
        {
            var container = Container.Default;

            Assert.IsInstanceOfType(container, typeof(Container));
        }

        [TestMethod]
        public void Default_Set_MaxInstantiationDepth()
        {
            var container = Container.Default;
            container.MaxInstantiationDepth = 6;
            Assert.AreEqual(6u, container.MaxInstantiationDepth);
        }

        [TestMethod]
        public void Resolve_ValueType()
        {
            var container = Container.Default;
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<int>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_String()
        {
            var container = Container.Default;
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<string>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_Delegate()
        {
            var container = Container.Default;
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<Container.Creator>();
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Resolve_ActionDelegate()
        {
            var container = Container.Default;
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
            var container = Container.Default;
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
            var container = Container.Default;
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
            var container = Container.Default;
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                var value = container.Resolve<TestClassBase>();
                Assert.IsNull(value);
                Assert.Fail("Should have thrown a ContainerResolutionException exception.");
            });
        }

        [TestMethod]
        public void Default_GetTwice_ReturnsSameInstance()
        {
            var container1 = Container.Default;
            var container2 = Container.Default;

            Assert.AreSame(container1, container2);
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
        public void CanResolve_BoundGenericTypeWithoutRegistered_ReturnsFalse()
        {
            var container = new Container();

            var testing = container.CanResolve<GenericClassWithInterface<int, string>>();

            Assert.IsFalse(testing);
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
        public void CanResolve_ClassNamedWithLazyFactoryDependency_ReturnsFalse()
        {
            var container = new Container();

            var result = container.CanResolve<TestClassWithNamedLazyFactory>();

            Assert.IsFalse(result);
        }

        //[TestMethod]
        //public void LazyFactory_CalledByDependantClass_ReturnsInstanceOfType()
        //{
        //    var container = new Container();
        //    var exceptionThrown = Assert.ThrowsException<ContainerResolutionException>(() =>
        //        {
        //            var item = container.Resolve<TestClassWithLazyFactory>();

        //            item.Method1();

        //            Assert.IsInstanceOfType(item.Prop1, typeof(TestClassDefaultCtor));
        //        });

        //    Assert.IsInstanceOfType(exceptionThrown, typeof(ContainerResolutionException));
        //}

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
        public void BuildUp_ObjectWithPropertyDependenciesAndDepsRegistered_SetsDependencies()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();
            container.Register<ITestInterface2, TestClass2>();
            var input = new TestClassPropertyDependencies();

            container.ResolveMembers(input, true);

            Assert.IsNotNull(input.Property1);
            Assert.IsNotNull(input.Property2);
            Assert.IsNotNull(input.ConcreteProperty);
        }

        [TestMethod]
        public void BuildUp_ObjectWithPropertyDependenciesAndDepsNotRegistered_DoesNotThrow()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();
            var input = new TestClassPropertyDependencies();

            container.ResolveMembers(input, true);

            Assert.IsNotNull(input.Property1);
            Assert.IsNull(input.Property2);
        }

        [TestMethod]
        public void BuildUp_ObjectWithPropertyDependenciesWithSomeSet_SetsOnlyUnsetDependencies()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();
            container.Register<ITestInterface2, TestClass2>();
            var preset = new TestClassDefaultCtor();
            var input = new TestClassPropertyDependencies();
            input.Property1 = preset;

            container.ResolveMembers(input);

            Assert.AreSame(preset, input.Property1);
            Assert.IsNotNull(input.Property2);
        }

        [TestMethod]
        public void BuildUp_ObjectWithPropertyDependenciesAndDepsRegistered_DoesNotSetWriteOnlyProperty()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();
            container.Register<ITestInterface2, TestClass2>();
            var input = new TestClassPropertyDependencies();

            container.ResolveMembers(input);

            Assert.IsNull(input.WriteOnlyProperty);
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

        #region Unregister

        // private readonly ResolveOptions options = ResolveOptions.FailUnregisteredAndNameNotFound;
        #region Unregister With Implementation

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

        #endregion

        #region Unregister With Interface

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

        #endregion

        #region Unregister<T> With Implementation

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

        #endregion

        #region Unregister<T> With Interface

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

        #endregion

        #endregion

        [TestMethod]
        public void CreateApplicationObject()
        {
            var container = new Container();
            container.Register<IRepository, MockRepository>();
            var app = container.Resolve<MockApplication>();
            Assert.IsNotNull(app);
            Assert.IsInstanceOfType(app, typeof(MockApplication));
            Assert.IsNotNull(app.Repository);
            Assert.IsInstanceOfType(app.Repository, typeof(IRepository));
            Assert.IsInstanceOfType(app.Repository, typeof(MockRepository));
        }

        [TestMethod]
        public void CreateNonResolveableObject()
        {
            var container = new Container();
            MockNonResolveableObject app = null;
            try
            {
                app = container.Resolve<MockNonResolveableObject>();
                Assert.Fail("Should have thrown an ContainerResolutionException exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ContainerResolutionException));
                Assert.IsNull(app);
            }
        }

        [TestMethod]
        public void GetChildContainer()
        {
            var container = new Container();
            Assert.IsFalse(container.CanResolve<Container>());
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

        [TestMethod]
        public void CreateUnregisteredType()
        {
            var container = new Container();
            IRepository value = null;
            try
            {
                value = container.Resolve<IRepository>();
                Assert.Fail("Should have thrown a 'UnregisteredTypeException' exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ContainerResolutionException));
                Assert.IsNull(value);
            }
        }

        [TestMethod, TestCategory("Custom Args")]
        public void ProperCustomArgsProvided()
        {
            var container = new Container();
            var result = container.Resolve<TestClassWithStringAndIntParameters>(new object[] { "string", 4 });
            Assert.IsNotNull(result, "Result is null!");
            Assert.AreEqual(4, result.IntProperty);
            Assert.AreEqual("string", result.StringProperty);
        }

        [TestMethod, TestCategory("Custom Args")]
        public void WronglyOrderedCustomArgProvided()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                container.Resolve<TestClassWithStringAndIntParameters>(new object[] { 4, "string" });
            });
        }

        [TestMethod, TestCategory("Custom Args")]
        public void DifferentNumberOfArgsProvided()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                container.Resolve<TestClassWithStringAndIntParameters>(new object[] { false, "string", "bad-arg" });
            });
        }

        [TestMethod, TestCategory("Custom Args")]
        public void CustomArgsProvided_WithNullString()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                container.Resolve<TestClassWithStringAndIntParameters>(new object[] { null, 4 });
            });
        }

        [TestMethod, TestCategory("Custom Args")]
        public void CustomArgsNotProvided()
        {
            var container = new Container();
            Assert.ThrowsException<ContainerResolutionException>(() =>
            {
                object[] args = null;
                container.Resolve<TestClassWithStringAndIntParameters>(args);
            });
        }

        [TestMethod]
        public void SelfReferentialConstructor()
        {
            var container = new Container();
            Assert.ThrowsException<ExceededMaxInstantiationDepthException>(() =>
            {
                container.Resolve<TestClassConstructorSelfReferential>();
            });
        }

        [TestMethod]
        public void CircularReferenceConstructor()
        {
            var container = new Container();
            Assert.ThrowsException<ExceededMaxInstantiationDepthException>(() =>
            {
                container.Resolve<TestClassCircularReferenceA>();
            });
        }
    }
}
