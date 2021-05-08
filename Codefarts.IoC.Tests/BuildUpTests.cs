// <copyright file="BuildUpTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("BuildUp")]
    public class BuildUpTests
    {
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
    }
}