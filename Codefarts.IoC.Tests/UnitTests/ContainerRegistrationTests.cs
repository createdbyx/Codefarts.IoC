using System;
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
            container.Register<ITestInterface>(() => new TestClassDefaultCtor() { Prop1 = "Testing" });
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
        public void RegisterTwice()
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