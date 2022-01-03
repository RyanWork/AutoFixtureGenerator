using Application.Bridges;
using Application.Models;

namespace Application.Stores;

public class PersonStore
{
    private readonly PersonBridge _personBridge;

    private readonly ISomeOtherDependency _someOtherDependency;
    
    public PersonStore(PersonBridge personBridge, ISomeOtherDependency someOtherDependency)
    {
        _personBridge = personBridge;
        _someOtherDependency = someOtherDependency;
    }

    public IEnumerable<Person> GetEveryone()
    {
        IList<Person> allPeopleAndMore = new List<Person>(_personBridge.GetAllPeople());
        allPeopleAndMore.Add(new Person("Ryan", "Ha"));
        return allPeopleAndMore;
    }
}