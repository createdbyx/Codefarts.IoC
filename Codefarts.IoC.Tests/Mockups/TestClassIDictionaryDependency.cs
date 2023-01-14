using System.Collections;
using System.Collections.Generic;

namespace Codefarts.IoC.Tests;

public class TestClassIDictionaryDependency
{
    public IDictionary Items { get; private set; }

    public int ItemCount
    {
        get
        {
            return this.Items == null ? 0 : this.Items.Count;
        }
    }

    public TestClassIDictionaryDependency(IDictionary dic)
    {
        this.Items = dic;
    }
}

public class TestClassGenericIDictionaryDependency
{
    public IDictionary<string, int> Items { get; private set; }

    public int ItemCount
    {
        get
        {
            return this.Items == null ? 0 : this.Items.Count;
        }
    }

    public TestClassGenericIDictionaryDependency(IDictionary<string, int> dic)
    {
        this.Items = dic;
    }
}

public class TestClassGenericIDictionaryDependency2<T1, T2>
{
    public IDictionary<T1, T2> Items { get; private set; }

    public int ItemCount
    {
        get
        {
            return this.Items == null ? 0 : this.Items.Count;
        }
    }

    public TestClassGenericIDictionaryDependency2(IDictionary<T1, T2> dic)
    {
        this.Items = dic;
    }
}