// <copyright file="TinyIoCComparisonTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Codefarts.IoC.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("TinyIoC")]
    public class TinyIoCComparisonTests
    {

        [TestMethod]
        public void IEnumrable()
        {
            var tiny = new TinyIoC.TinyIoCContainer();
            tiny.Register<IEnumerable<ITestInterface>, List<ITestInterface>>();
            var result = tiny.Resolve<TestClassEnumerableDependency>();

        }
    }
}