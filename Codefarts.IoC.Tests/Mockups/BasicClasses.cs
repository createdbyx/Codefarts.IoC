namespace Codefarts.IoC.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Codefarts.IoC;

    public interface ITestInterface
    {
    }

    internal interface ITestMultiDeps
    {
    }

    public class TestClassDefaultCtor : ITestInterface
    {
        public string Prop1 { get; set; }

        public TestClassDefaultCtor()
        {
        }

        public static ITestInterface CreateNew(Container container)
        {
            return new TestClassDefaultCtor() { Prop1 = "Testing" };
        }
    }

    internal interface ITestInterface2
    {
    }

    internal class TestClass2 : ITestInterface2
    {
    }

    internal class TestClassWithContainerDependency
    {
        public Container _Container { get; private set; }

        public TestClassWithContainerDependency(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this._Container = container;
        }
    }

    public class TestClassWithInterfaceDependency : ITestInterface2
    {
        public ITestInterface Dependency { get; set; }

        public int Param1 { get; private set; }

        public string Param2 { get; private set; }

        public TestClassWithInterfaceDependency(ITestInterface dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException("dependency");

            this.Dependency = dependency;
        }

        public TestClassWithInterfaceDependency(ITestInterface dependency, int param1, string param2)
        {
            this.Dependency = dependency;
            this.Param1 = param1;
            this.Param2 = param2;
        }
    }

    internal class TestClassWithAbstractDependency
    {
        public TestClassBase Prop1 { get; set; }

        public TestClassWithAbstractDependency(TestClassBase arg)
        {
            this.Prop1 = arg;
        }
    }

    internal class TestClassWithDependency
    {
        public TestClassDefaultCtor Dependency { get; set; }

        public int Param1 { get; private set; }

        public string Param2 { get; private set; }

        public TestClassWithDependency(TestClassDefaultCtor dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException("dependency");

            this.Dependency = dependency;
        }

        public TestClassWithDependency(TestClassDefaultCtor dependency, int param1, string param2)
        {
            this.Param1 = param1;
            this.Param2 = param2;
        }
    }

    internal class TestClassPrivateCtor
    {
        private TestClassPrivateCtor()
        {
        }
    }

    internal class TestClassWithStringAndIntParameters
    {
        public string StringProperty { get; set; }

        public int IntProperty { get; set; }

        public TestClassWithStringAndIntParameters(string stringProperty, int intProperty)
        {
            this.StringProperty = stringProperty;
            this.IntProperty = intProperty;
        }
    }

    internal class TestClassWithStringParameter
    {
        public string StringProperty { get; set; }

        public TestClassWithStringParameter(string stringProperty)
        {
            this.StringProperty = stringProperty;
        }
    }

    internal class TestClassWithDependencyAndParameters
    {
        TestClassDefaultCtor Dependency { get; set; }

        public int Param1 { get; private set; }

        public string Param2 { get; private set; }

        public TestClassWithDependencyAndParameters(TestClassDefaultCtor dependency, int param1, string param2)
        {
            if (dependency == null)
                throw new ArgumentNullException("dependency");

            this.Dependency = dependency;
            this.Param1 = param1;
            this.Param2 = param2;
        }
    }

    internal class TestClassNoInterfaceDefaultCtor
    {
        public TestClassNoInterfaceDefaultCtor()
        {
        }
    }

    internal class TestClassNoInterfaceDependency
    {
        public ITestInterface Dependency { get; set; }

        public TestClassNoInterfaceDependency(ITestInterface dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException("dependency");

            this.Dependency = dependency;
        }
    }

    public class DisposableTestClassWithInterface : IDisposable, ITestInterface
    {
        public void Dispose()
        {
        }
    }

    public class GenericClassWithInterface<I, S> : ITestInterface
    {
        public I Prop1 { get; set; }

        public S Prop2 { get; set; }

        public GenericClassWithInterface()
        {
        }

        public GenericClassWithInterface(I prop1, S prop2)
        {
            this.Prop1 = prop1;
            this.Prop2 = prop2;
        }
    }


    internal class GenericClassWithParametersAndDependencies<I, S>
    {
        public ITestInterface2 Dependency { get; private set; }

        public I Prop1 { get; set; }

        public S Prop2 { get; set; }

        public GenericClassWithParametersAndDependencies(ITestInterface2 dependency)
        {
            this.Dependency = dependency;
        }

        public GenericClassWithParametersAndDependencies(ITestInterface2 dependency, I prop1, S prop2)
        {
            this.Dependency = dependency;
            this.Prop1 = prop1;
            this.Prop2 = prop2;
        }
    }

    internal class TestClassWithLazyFactory
    {
        private Func<TestClassDefaultCtor> _Factory;

        public TestClassDefaultCtor Prop1 { get; private set; }

        public TestClassDefaultCtor Prop2 { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestClassWithLazyFactory class.
        /// </summary>
        /// <param name="factory"></param>
        public TestClassWithLazyFactory(Func<TestClassDefaultCtor> factory)
        {
            this._Factory = factory;
        }

        public void Method1()
        {
            this.Prop1 = this._Factory.Invoke();
        }

        public void Method2()
        {
            this.Prop2 = this._Factory.Invoke();
        }
    }

    internal class TestClassWithNamedLazyFactory
    {
        private Func<string, TestClassDefaultCtor> _Factory;

        public TestClassDefaultCtor Prop1 { get; private set; }

        public TestClassDefaultCtor Prop2 { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestClassWithLazyFactory class.
        /// </summary>
        /// <param name="factory"></param>
        public TestClassWithNamedLazyFactory(Func<string, TestClassDefaultCtor> factory)
        {
            this._Factory = factory;
        }

        public void Method1()
        {
            this.Prop1 = this._Factory.Invoke("Testing");
        }

        public void Method2()
        {
            this.Prop2 = this._Factory.Invoke(string.Empty);
        }
    }

    internal class TestClassWithNameAndParamsLazyFactory
    {
        private Func<string, IDictionary<string, object>, TestClassWithStringAndIntParameters> _Factory;

        public TestClassWithStringAndIntParameters Prop1 { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestClassWithNameAndParamsLazyFactory class.
        /// </summary>
        /// <param name="factory"></param>
        public TestClassWithNameAndParamsLazyFactory(Func<string, IDictionary<string, object>, TestClassWithStringAndIntParameters> factory)
        {
            this._Factory = factory;
            this.Prop1 = this._Factory.Invoke(
                string.Empty, new Dictionary<string, object> { { "stringProperty", "Testing" }, { "intProperty", 22 } });
        }
    }

    internal class TestClassMultiDepsMultiCtors
    {
        public TestClassDefaultCtor Prop1 { get; private set; }

        public TestClassDefaultCtor Prop2 { get; private set; }

        public int NumberOfDepsResolved { get; private set; }

        public TestClassMultiDepsMultiCtors(TestClassDefaultCtor prop1)
        {
            this.Prop1 = prop1;
            this.NumberOfDepsResolved = 1;
        }

        public TestClassMultiDepsMultiCtors(TestClassDefaultCtor prop1, TestClassDefaultCtor prop2)
        {
            this.Prop1 = prop1;
            this.Prop2 = prop2;
            this.NumberOfDepsResolved = 2;
        }
    }

    internal class TestClassMultiInterfaceDepsMultiCtors : ITestMultiDeps
    {
        public ITestInterface Prop1 { get; private set; }

        public ITestInterface Prop2 { get; private set; }

        public int NumberOfDepsResolved { get; private set; }

        public TestClassMultiInterfaceDepsMultiCtors(ITestInterface prop1)
        {
            this.Prop1 = prop1;
            this.NumberOfDepsResolved = 1;
        }

        public TestClassMultiInterfaceDepsMultiCtors(ITestInterface prop1, ITestInterface prop2)
        {
            this.Prop1 = prop1;
            this.Prop2 = prop2;
            this.NumberOfDepsResolved = 2;
        }
    }

    internal class TestClassConstructorSelfReferential
    {
        /// <summary>
        /// Initializes a new instance of the TestClassConstructorFailure class.
        /// </summary>
        public TestClassConstructorSelfReferential(TestClassConstructorSelfReferential self)
        {
        }
    }

    internal class TestClassCircularReferenceA
    {
        /// <summary>
        /// Initializes a new instance of the TestClassConstructorFailure class.
        /// </summary>
        public TestClassCircularReferenceA(TestClassCircularReferenceB self)
        {
        }
    }

    internal class TestClassCircularReferenceB
    {
        /// <summary>
        /// Initializes a new instance of the TestClassConstructorFailure class.
        /// </summary>
        public TestClassCircularReferenceB(TestClassCircularReferenceC self)
        {
        }
    }

    internal class TestClassCircularReferenceC
    {
        /// <summary>
        /// Initializes a new instance of the TestClassConstructorFailure class.
        /// </summary>
        public TestClassCircularReferenceC(TestClassCircularReferenceA self)
        {
        }
    }

    internal class TestClassConstructorFailure
    {
        /// <summary>
        /// Initializes a new instance of the TestClassConstructorFailure class.
        /// </summary>
        public TestClassConstructorFailure()
        {
            throw new NotImplementedException();
        }
    }

    internal abstract class TestClassBase
    {
    }

    internal class TestClassWithBaseClass : TestClassBase
    {
    }

    internal class TestClassPropertyDependencies
    {
        public ITestInterface Property1 { get; set; }

        public ITestInterface2 Property2 { get; set; }

        public int Property3 { get; set; }

        public string Property4 { get; set; }

        public TestClassDefaultCtor ConcreteProperty { get; set; }

        public ITestInterface ReadOnlyProperty { get; private set; }

        public ITestInterface2 WriteOnlyProperty { internal get; set; }

        /// <summary>
        /// Initializes a new instance of the TestClassPropertyDependencies class.
        /// </summary>
        public TestClassPropertyDependencies()
        {
        }
    }

    public class TestClassArrayDependency
    {
        public ITestInterface[] Items { get; private set; }

        public int ItemCount
        {
            get
            {
                return this.Items == null ? 0 : this.Items.Length;
            }
        }

        public TestClassArrayDependency(ITestInterface[] array)
        {
            this.Items = array;
        }
    }
    
    public class TestClassEnumerableDependency
    {
        public IEnumerable<ITestInterface> Enumerable { get; private set; }

        public int EnumerableCount
        {
            get
            {
                return this.Enumerable == null ? 0 : this.Enumerable.Count();
            }
        }

        public TestClassEnumerableDependency(IEnumerable<ITestInterface> enumerable)
        {
            this.Enumerable = enumerable;
        }
    }

    internal class TestClassEnumerableDependency2
    {
        public IEnumerable<ITestInterface2> Enumerable { get; private set; }

        public int EnumerableCount
        {
            get
            {
                return this.Enumerable == null ? 0 : this.Enumerable.Count();
            }
        }

        public TestClassEnumerableDependency2(IEnumerable<ITestInterface2> enumerable)
        {
            this.Enumerable = enumerable;
        }
    }

    public interface IThing<T> where T : new()
    {
        T Get();
    }

    public class DefaultThing<T> : IThing<T> where T : new()
    {
        public T Get()
        {
            return new T();
        }
    }
}