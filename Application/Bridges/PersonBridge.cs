using Application.Models;

namespace Application.Bridges;

public class PersonBridge
{
    private readonly ISomeOtherDependency _someOtherDependency;

    public PersonBridge(ISomeOtherDependency someOtherDependency)
    {
        _someOtherDependency = someOtherDependency;
    }
    
    public IEnumerable<Person> GetAllPeople()
    {
        for (int i = 0; i < 10; i++)
        {
            Person person = new(_someOtherDependency.SomeMethod(), _someOtherDependency.SomeMethod());
            yield return person;
        }
    }
}