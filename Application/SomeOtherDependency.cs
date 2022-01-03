namespace Application;

public class SomeOtherDependency : ISomeOtherDependency
{
    public string SomeMethod() => "Hello world";
}

public interface ISomeOtherDependency
{
    public string SomeMethod();
}