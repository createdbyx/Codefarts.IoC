// <copyright file="MaxInstantiationDepthTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("MaxInstantiationDepth")]
    public class MaxInstantiationDepthTests
    {
        [TestMethod]
        public void Default_Set_MaxInstantiationDepth()
        {
            var container = new Container();
            container.MaxInstantiationDepth = 6;
            Assert.AreEqual(6u, container.MaxInstantiationDepth);
        }

        [TestMethod]
        public void MaxInstantiationDepth_Zero_NoDependencies()
        {
            var container = new Container();
            container.MaxInstantiationDepth = 0;
            var value = container.Resolve<TestClassWithBaseClass>();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void MaxInstantiationDepth_Zero_OneDependency()
        {
            var container = new Container();
            container.MaxInstantiationDepth = 0;
            Assert.ThrowsException<ExceededMaxInstantiationDepthException>(() =>
            {
                var value = container.Resolve<TestClassWithDependency>();
                Assert.IsNull(value);
            });
        }

        [TestMethod]
        public void MaxInstantiationDepth_One_OneDependency()
        {
            var container = new Container();
            container.MaxInstantiationDepth = 1;
            var value = container.Resolve<TestClassWithDependency>();
            Assert.IsNotNull(value);
            Assert.IsNotNull(value.Dependency);
        }
    }
}