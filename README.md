
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


## Important Notes
* It may be important to invalidate your IDE cache when attempting to rebuild the *.g.cs file. The file may be cached in memory so cleaning the solution does not always work.