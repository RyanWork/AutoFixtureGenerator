namespace Application.Stores;

public class SomeOtherStore : ISomeOtherStore
{
    public string SomeMethod() => "Hello world";
}

public interface ISomeOtherStore
{
    public string SomeMethod();
}