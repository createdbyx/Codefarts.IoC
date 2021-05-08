﻿// <copyright file="TinyIoCComparisonTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

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
            var result = tiny.Resolve<TestClassEnumerableDependency>();

        }
    }
}