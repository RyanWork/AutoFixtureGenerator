using AutoFixture;
using AutoFixture.AutoMoq;

namespace Application.Test
{
    public partial class TestBase
    {
        protected readonly IFixture Fixture;

        protected TestBase()
        {
            Fixture = new Fixture();
            // .Customize(new AutoMoqCustomization());
        }

        partial void PopulateFixture();
    }
}