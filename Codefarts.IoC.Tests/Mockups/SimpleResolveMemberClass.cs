using System;

namespace Codefarts.IoC.Tests;

public class SimpleResolveMemberClass
{
    public TestClassDefaultCtor TestClassProperty { get; set; }
    public TestClassDefaultCtor TestClassField;
}

public class SimpleResolveMemberClassWithUnresolvableField
{
    public TestClassDefaultCtor TestClassProperty { get; set; }
    public TestClassWithInterfaceDependency TestClassField;
}

public class SimpleClassCtorThrowsExeption
{
    public SimpleClassCtorThrowsExeption()
    {
        throw new Exception();
    }

    public SimpleClassCtorThrowsExeption(bool throwException)
    {
    }

    public TestClassDefaultCtor TestClassProperty { get; set; }
    public TestClassWithInterfaceDependency TestClassField;
}

public class SimpleSinglePropertyClass
{
    public string Value { get; set; }
}