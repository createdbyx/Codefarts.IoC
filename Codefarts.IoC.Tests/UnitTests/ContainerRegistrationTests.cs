using System;
using System.CodeDom;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codefarts.IoC.Tests
{
    [TestCategory("Registration")]
    [TestClass]
    public class ContainerRegistrationTests
    {
        [TestMethod]
        public void Register_InterfaceAndImplementation_CanRegister()
        {
            var container = new Container();
            container.Register<ITestInterface, TestClassDefaultCtor>();
        }

        [TestMethod]
        public void Register_GenericTypeWithInterface_CanRegister()
        {
            var container = new Container();

            container.Register<ITestInterface, GenericClassWithInterface<int, string>>();
        }

        [TestMethod]
        public void Register_Instance_CanRegister()
        {
            var container = new Container();
            container.Register<DisposableTestClassWithInterface>(() => new DisposableTestClassWithInterface());
        }

        [TestMethod]
        public void Register_InstanceUsingInterface_CanRegister()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => new DisposableTestClassWithInterface());
        }

        [TestMethod]
        public void Register_WithDelegateFactoryStaticMethod_CanRegister()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => TestClassDefaultCtor.CreateNew(container));
        }

        [TestMethod]
        public void Register_WithDelegateFactoryLambda_CanRegister()
        {
            var container = new Container();
            container.Register<ITestInterface>(() => new TestClassDefaultCtor() {Prop1 = "Testing"});
        }

        [TestMethod]
        public void RegisterNonGeneric_BasicType_RegistersAndCanResolve()
        {
            var container = new Container();

            //container.Register(typeof(TestClassDefaultCtor));
            var result = container.Resolve<TestClassDefaultCtor>();

            Assert.IsInstanceOfType(result, typeof(TestClassDefaultCtor));
        }

        [TestMethod]
        public void RegisterNonGeneric_TypeImplementingInterface_RegistersAndCanResolve()
        {
            var container = new Container();

            container.Register(typeof(ITestInterface), typeof(TestClassDefaultCtor));
            var result = container.Resolve<ITestInterface>();

            Assert.IsInstanceOfType(result, typeof(ITestInterface));
        }

        [TestMethod]
        public void RegisterConcreteTwice()
        {
            var container = new Container();
            try
            {
                container.Register<IRepository, MockRepository>();
                container.Register<IRepository, MockRepository>();
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
            }
        }

        [TestMethod]
        public void RegisterCreatorTwice()
        {
            var container = new Container();
            try
            {
                container.Register<IRepository>(() => new MockRepository());
                Assert.IsTrue(container.CanResolve<IRepository>());
                container.Register<IRepository>(() => new MockRepository());
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
            }
        }

        [TestMethod]
        public void RegisterMatchingTypes()
        {
            var container = new Container();
            try
            {
                container.Register<MockRepository, MockRepository>();
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidAbstractConcreteType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(IRepository), typeof(TestClassBase));
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidInterfaceConcreteType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(IRepository), typeof(ITestInterface));
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidValueTypeConcreteType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(IRepository), typeof(int));
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidValueTypeKeyType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(int), typeof(TestClassWithBaseClass));
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidActionDelegateConcreteType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(IRepository), typeof(Action));
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidActionDelegateKeyType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(Action), typeof(TestClassWithBaseClass));
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidStringConcreteType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(IRepository), typeof(string));
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidStringKeyType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(string), typeof(TestClassWithBaseClass));
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterConcreteTypeCanNotBeCreatedFromKeyType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(ITestInterface), typeof(TestClassWithBaseClass));
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterNullKeyValidConcreteType()
        {
            var container = new Container();
            try
            {
                container.Register(null, typeof(MockRepository));
                Assert.Fail("Should have thrown a 'ArgumentNullException' exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
            }
        }

        [TestMethod]
        public void RegisterValidKeyNullConcreteType()
        {
            var container = new Container();
            try
            {
                Type nullType = null;
                container.Register(typeof(IRepository), nullType);
                Assert.Fail("Should have thrown a 'ArgumentNullException' exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
            }
        }

        [TestMethod]
        public void RegisterInvalidStringValidConcreteType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(string), typeof(MockRepository));
                Assert.Fail("Should have thrown a 'RegistrationException' exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidValueTypeValidConcreteType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(int), typeof(MockRepository));
                Assert.Fail("Should have thrown a 'RegistrationException' exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterInvalidActionKeyValidConcreteType()
        {
            var container = new Container();
            try
            {
                container.Register(typeof(Action), typeof(MockRepository));
                Assert.Fail("Should have thrown a 'RegistrationException' exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(RegistrationException));
            }
        }

        [TestMethod]
        public void RegisterNullCreator()
        {
            var container = new Container();
            try
            {
                container.Register<IRepository>(null);
                Assert.Fail("Should have thrown a 'ArgumentNullException' exception.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
            }
        }

        [TestMethod]
        public void RegisterNullTypeAndValidCreator()
        {
            var container = new Container();

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var creator = new Container.Creator(() => string.Empty);
                container.Register(null, creator);

                Assert.Fail("Should have thrown a 'ArgumentNullException' exception.");
            });
        }

        [TestMethod]
        public void RegisterOnce()
        {
            var container = new Container();
            container.Register<IRepository>(() => new MockRepository());
            Assert.IsTrue(container.CanResolve<IRepository>());
            var value = container.Resolve<IRepository>();
            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(IRepository));
            Assert.IsInstanceOfType(value, typeof(MockRepository));
        }

        [TestMethod]
        public void RegisterSingleton()
        {
            var container = new Container();
            container.Register<Container>(() => container);
            Assert.IsTrue(container.CanResolve<Container>());
            var value = container.Resolve<Container>();
            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(Container));
            Assert.AreSame(value, container);
        }

        [TestMethod]
        public void RegisterOnceWithoutCallbackFunc()
        {
            var container = new Container();
            container.Register<IRepository, MockRepository>();
            Assert.IsTrue(container.CanResolve<IRepository>());
            var value = container.Resolve<IRepository>();
            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(IRepository));
            Assert.IsInstanceOfType(value, typeof(MockRepository));
        }
    }
}