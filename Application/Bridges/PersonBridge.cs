using Application.Models;
using Application.Stores;

namespace Application.Bridges;

public class PersonBridge
{
    public string TestProperty { get; set; } = "Hello";

    private readonly ISomeOtherStore _someOtherStore;
    
    public PersonBridge(ISomeOtherStore someOtherStore)
    {
        _someOtherStore = someOtherStore;
    }
    
    public IEnumerable<Person> GetAllPeople()
    {
        for (int i = 0; i < 10; i++)
        {
            Person person = new(_someOtherStore.SomeMethod(), _someOtherStore.SomeMethod());
            yield return person;
        }
    }
}