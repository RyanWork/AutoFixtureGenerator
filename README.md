
# AutoFixtureGenerator
Project using Source Generators to automatically pull objects from an assembly and wrap them in `Mock<T>` and `Lazy<T>` and inject/freeze that instance directly into an AutoFixture instance.



## Usage
The project that has a dependency on AutoFixture should also have a dependency on the AutoFixtureGenerator project. The following should be added to your *.csproj file:
```xml
<ItemGroup>
	<ProjectReference Include="..\AutoFixtureGenerator\AutoFixtureGenerator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="true" />
</ItemGroup>
```
A partial class should be declared in which a partial implementation of `PopulateFixture()` is provided and an AutoFixture instance is also generated.

```csharp
public partial class TestBase  
{  
    protected readonly IFixture Fixture;  
  
    protected TestBase()  
    {  
        Fixture = new Fixture()  
            .Customize(new AutoMoqCustomization());  
        PopulateFixture();  
    }  
  
    partial void PopulateFixture();  
}
```

## Debugging
In the AutoFixtureGenerator class and the `Initialize()` method, you may uncomment the `#if DEBUG` directive to launch a debugger.
```csharp
[Generator]  
public class AutoFixtureGenerator : ISourceGenerator  
{  
      public void Initialize(GeneratorInitializationContext context)  
      {  
// #if DEBUG  
//           if (!Debugger.IsAttached)  
//           {  
//               Debugger.Launch();  
//           }  
// #endif  
      }
}
  ```

## Benchmarks
Due to the nature of proprietary software, I have not included all of the source code for benchmarking. However, I will share some reasonable pieces of code in which some implementation could be interpreted as well as an explanation of the difference between the two benchmarked results:

The following code snippet compares the times taken to generate a useable `IFixture` instance. `TestUtils.BaseFixture` uses reflection to determine what `Mock<T>`/`Lazy<T>` instances it needs to freeze and inject. The second test just instantiates a `new TestBase()` in which an `IFixture` instance lives. Both contain the most minimal implementation to extract values that it should create.
```csharp
using Application.Test;  
using AutoFixture;  
using BenchmarkDotNet.Attributes;  
using BenchmarkDotNet.Running;  
  
namespace Application.Performance;  
  
public static class Program  
{  
    public class AutoFixturePopulate  
    {  
		[Benchmark]  
		public IFixture UsingReflection() => TestUtils.BaseFixture();  
  
		[Benchmark]  
		public TestBase UsingSourceGeneratedValues() => new TestBase();
    }  
  
    public static void Main(string[] args)  
    {  
        BenchmarkRunner.Run(typeof(Program).Assembly);  
    }  
}
```


### Results
Comparing the two results, we can see a ~60% increase in average time taken to generate an `IFixture` instance. As the source generated version completely removes the need for reflection, we would expect a sizeable performance improvement.

|  Method |     Mean |     Error |    StdDev |
|-------- |---------:|----------:|----------:|
| UsingReflection| 1.974 ms | 0.0164 ms | 0.0137 ms |
|     UsingSourceGeneratedValues | 1.212 ms | 0.0029 ms | 0.0023 ms |

We should also make note of the fact that using source generated values is much better from a scaling perspective. Assume that we have an `# of objects --> ∞` we need to inject into our `IFixture` instance. Every time a reflection call needs to be done, a lot of overhead is generated just to pull an object from the assembly. Using the source generated values, we determine exactly what we need to inject at compile time to remove reflection. 

Moreover, assume we also have `# of tests --> ∞`. The time taken to complete an entire test suite will not scale well either as the method `UsingReflection` gets slower, the entire test suite also slows down by `(# of tests) * (slow down factor)`. This can be detrimental to continuous integration and developer experience in general.

## Important Notes
* It may be important to invalidate your IDE cache when attempting to rebuild the *.g.cs file. The file may be cached in memory so cleaning the solution does not always work.