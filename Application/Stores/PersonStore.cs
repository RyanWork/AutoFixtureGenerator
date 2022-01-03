using Application.Bridges;
using Application.Models;

namespace Application.Stores;

public class PersonStore
{
    private readonly PersonBridge _personBridge;

    private readonly ISomeOtherStore _someOtherStore;
    
    public PersonStore(PersonBridge personBridge, ISomeOtherStore someOtherStore)
    {
        _personBridge = personBridge;
        _someOtherStore = someOtherStore;
    }

    public IEnumerable<Person> GetEveryone()
    {
        IList<Person> allPeopleAndMore = new List<Person>(_personBridge.GetAllPeople());
        allPeopleAndMore.Add(new Person("Ryan", "Ha"));
        return allPeopleAndMore;
    }
}