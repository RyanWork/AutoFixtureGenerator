using Application.Models;
using Application.Stores;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.Stores;

public class PersonStoreTests : TestBase
{
    private readonly PersonStore _sut;

    private readonly Mock<ISomeOtherStore> _mockSomeOtherStore;

    public PersonStoreTests()
    {
        _mockSomeOtherStore = Fixture.Create<Mock<ISomeOtherStore>>();
        _sut = Fixture.Create<PersonStore>();
    }

    [Fact]
    public void GetEveryone_DefaultParameters_ReturnsRandomListOfTenAndRyanHa()
    {
        _mockSomeOtherStore.Setup(x => x.SomeMethod())
            .Returns("This method has been setup!");
        
        var result = _sut.GetEveryone();

        result.Should()
            .NotBeNullOrEmpty()
            .And.HaveCount(11)
            .And.Contain(new Person("Ryan", "Ha"));
    }
}