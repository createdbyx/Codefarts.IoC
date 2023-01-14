// <copyright file="GeneralTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC.Tests
{
    using System;
    using Codefarts.IoC;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("General")]
    public partial class GeneralTests
    {
        [TestMethod]
        public void Default_Get_ReturnsInstanceOfContainer()
        {
            var container = new Container();

            Assert.IsInstanceOfType(container, typeof(Container));
        }

        [TestMethod]
        public void Default_GetTwice_ReturnsSameInstance()
        {
            var container1 = Container.Default;
            var container2 = Container.Default;

            Assert.AreSame(container1, container2);
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

        // private readonly ResolveOptions options = ResolveOptions.FailUnregisteredAndNameNotFound;

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
            Assert.IsTrue(container.CanResolve<Container>());
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

        [TestMethod]
        public void SelfReferentialConstructor()
        {
            var container = new Container();
            var ex = Assert.ThrowsException<ContainerResolutionException>(() => { container.Resolve<TestClassConstructorSelfReferential>(); });
            Assert.IsInstanceOfType(ex.InnerException, typeof(ExceededMaxInstantiationDepthException));
        }

        [TestMethod]
        public void CircularReferenceConstructor()
        {
            var container = new Container();
            var ex = Assert.ThrowsException<ContainerResolutionException>(() => { container.Resolve<TestClassCircularReferenceA>(); });
            Assert.IsInstanceOfType(ex.InnerException, typeof(ExceededMaxInstantiationDepthException));
        }
    }
}