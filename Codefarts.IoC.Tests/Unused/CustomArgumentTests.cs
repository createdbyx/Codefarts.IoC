// <copyright file="CustomArgumentTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("Custom Arguments")]
    public class CustomArgumentTests
    {
        // [TestMethod]
        // public void ProperCustomArgsProvided()
        // {
        //     var container = new Container();
        //     var result = container.Resolve<TestClassWithStringAndIntParameters>(new object[] { "string", 4 });
        //     Assert.IsNotNull(result, "Result is null!");
        //     Assert.AreEqual(4, result.IntProperty);
        //     Assert.AreEqual("string", result.StringProperty);
        // }
        //
        // [TestMethod]
        // public void WronglyOrderedCustomArgProvided()
        // {
        //     var container = new Container();
        //     Assert.ThrowsException<ContainerResolutionException>(() =>
        //     {
        //         container.Resolve<TestClassWithStringAndIntParameters>(new object[] { 4, "string" });
        //     });
        // }
        //
        // [TestMethod]
        // public void DifferentNumberOfArgsProvided()
        // {
        //     var container = new Container();
        //     Assert.ThrowsException<ContainerResolutionException>(() =>
        //     {
        //         container.Resolve<TestClassWithStringAndIntParameters>(new object[] { false, "string", "bad-arg" });
        //     });
        // }
        //
        // [TestMethod]
        // public void CustomArgsProvided_WithNullString()
        // {
        //     var container = new Container();
        //     Assert.ThrowsException<ContainerResolutionException>(() =>
        //     {
        //         container.Resolve<TestClassWithStringAndIntParameters>(new object[] { null, 4 });
        //     });
        // }
        //
        // [TestMethod]
        // public void CustomArgsNotProvided()
        // {
        //     var container = new Container();
        //     Assert.ThrowsException<ContainerResolutionException>(() =>
        //     {
        //         object[] args = null;
        //         container.Resolve<TestClassWithStringAndIntParameters>(args);
        //     });
        // }
    }
}