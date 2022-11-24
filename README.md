Codefarts IoC is a Inversion of Control (IoC) library written C#

It's purpose is to provide a extremely simplified alternative to the other popular IoC libraries like TinyIoC, or Caliburn.Micro etc.

The core Container class that handles object instantiation is less then 600 lines of code including xml documentation comments! Contrast that with TinyIoC that contains over 4400 lines of sparsely commented code.

I hate code bloat and believe IoC containers that become monolithic are unnecessary and overkill.

### Features

- Uses single mechanism for instantiation in the form of callbacks
- Has MaxInstantiationDepth property you can set to prevent stack overflow exceptions then composing objects with deep nested dependencies.
- Legitimately tiny and simplistic code base that you can read through and understand.
- 123 unit tests and counting with almost full code coverage. 
- Can resolve object members like properties and fields.

### Known Issues

- May or may not handle multithreading properly. Needs a thorough code audit and stress test.
- Need to replace existing code to simplify and prevent stack overflow exceptions without needing to track stack call depth.
- Come up with a system for assigning a resolved objects properties and fields automatically. 
Currently have to do it by calling a separate method ResolveMembers on the object.

### Examples

Resolving objects.

    var container = new Container();

    // with generics
    value = container.Resolve<Stopwatch>();

    // default value to return if resolve fails
    value = container.Resolve<Stopwatch>(defaultValue); 

    // specify a type
    value = container.Resolve(typeof(Stopwatch)); 

Registration

    var container = new Container();
    
    // specify a generic type  
    container.Register<ISomeService, ServiceImplementaton>();

    // type with creator callback
    container.Register(typeof(Stopwatch), () => new Stopwatch());

    // type with creator callback that returns a singleton
    var stopwatch = new StopWatch();
    container.Register(typeof(Stopwatch), () => stopwatch);