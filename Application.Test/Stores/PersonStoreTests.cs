using Application.Bridges;
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

    public PersonStoreTests()
    {
        var test = Fixture.Create<PersonBridge>();
        var test2 = Fixture.Create<SomeOtherDependency>();
        var test3 = Fixture.Create<PersonStore>();
        _sut = Fixture.Create<PersonStore>();
        Fixture.Freeze<Mock<PersonBridge>>();
    }

    [Fact]
    public void GetEveryone_DefaultParameters_ReturnsRandomListOfTenAndRyanHa()
    {
        var result = _sut.GetEveryone();

        result.Should()
            .NotBeNullOrEmpty()
            .And.HaveCount(11)
            .And.Contain(new Person("Ryan", "Ha"));
    }
}