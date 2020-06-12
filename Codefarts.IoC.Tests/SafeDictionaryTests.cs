namespace Codefarts.IoC.Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, Obsolete("SafeDictionary Marked internal")]
    public class SafeDictionaryTests
    {
        [TestMethod]
        public void SetDisposableOnce()
        {
            var dic = new SafeDictionary<Type, Container.Creator>();
            var repo = new MockDisposableRepository();
            bool isDisposed = false;
            repo.WasDisposed = () => isDisposed = true;
            dic[typeof(MockDisposableRepository)] = () => repo;
            Assert.IsFalse(isDisposed);

            Container.Creator retrieved;
            Assert.IsTrue(dic.TryGetValue(typeof(MockDisposableRepository), out retrieved));
            Assert.IsNotNull(retrieved);
            var value = retrieved();
            Assert.AreSame(repo, value);
        }

        [TestMethod]
        public void ReplaceDisposable()
        {
            var dic = new SafeDictionary<Type, MockDisposableRepository>();
            var repo = new MockDisposableRepository();
            bool isDisposed = false;
            repo.WasDisposed = () => isDisposed = true;
            dic[typeof(MockDisposableRepository)] = repo;
            Assert.IsFalse(isDisposed);

            MockDisposableRepository retrieved;
            Assert.IsTrue(dic.TryGetValue(typeof(MockDisposableRepository), out retrieved));
            Assert.IsNotNull(retrieved);
            // var value = retrieved();
            Assert.AreSame(repo, retrieved);

            var newRepo = new MockDisposableRepository();
            dic[typeof(MockDisposableRepository)] = newRepo;
            Assert.IsTrue(isDisposed);
        }

        [TestMethod]
        public void KeysTest()
        {
            var dic = new SafeDictionary<Type, MockDisposableRepository>();
            var repo = new MockDisposableRepository();
            dic[typeof(MockDisposableRepository)] = repo;
            var keys = dic.Keys.ToArray();
            Assert.AreEqual(1, keys.Length);
            Assert.AreEqual(typeof(MockDisposableRepository), keys[0]);
        }

        [TestMethod]
        public void ClearTest()
        {
            var dic = new SafeDictionary<Type, MockDisposableRepository>();
            var repo = new MockDisposableRepository();
            dic[typeof(MockDisposableRepository)] = repo;
            var keys = dic.Keys.ToArray();
            Assert.AreEqual(1, keys.Length);
            Assert.AreEqual(typeof(MockDisposableRepository), keys[0]);
            dic.Clear();
            keys = dic.Keys.ToArray();
            Assert.AreEqual(0, keys.Length);
        }

        [TestMethod]
        public void DisposeTest()
        {
            var dic = new SafeDictionary<Type, MockDisposableRepository>();
            var repo = new MockDisposableRepository();
            bool isDisposed = false;
            repo.WasDisposed = () => isDisposed = true;
            using (dic)
            {
                dic[typeof(MockDisposableRepository)] = repo;
                Assert.IsFalse(isDisposed);

                MockDisposableRepository retrieved;
                Assert.IsTrue(dic.TryGetValue(typeof(MockDisposableRepository), out retrieved));
                Assert.IsNotNull(retrieved);
                Assert.AreSame(repo, retrieved);
            }

            Assert.IsTrue(isDisposed);  
        }
    }
}