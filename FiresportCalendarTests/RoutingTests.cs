using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
namespace FiresportCalendarTests
{
    public class RoutingTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public RoutingTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }
        [Theory]
        [InlineData("/")]
        [InlineData("/Identity/Account/Register")]
        [InlineData("/Identity/Account/Login")]
        public async Task PublicPages_Should_Return_OK(string URL)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });


            var response = await client.GetAsync(URL);
            var code = response.StatusCode;

            Assert.Equal(HttpStatusCode.OK, code);
        }

        [Theory]
        [InlineData("/Event/Index")]
        [InlineData("/Event/Create")]
        [InlineData("/Event/Edit")]
        [InlineData("/Event/Delete")]
        [InlineData("/Team/TimerRaces")]
        [InlineData("/Team/Detail/1")]
        [InlineData("/Team/Index")]
        //[InlineData("/Team/Create")]
        //[InlineData("/Team/Edit")]
        [InlineData("/Race/Index")]
        [InlineData("/Race/Create")]
        [InlineData("/Race/Detail")]
        [InlineData("/Race/Edit")]
        [InlineData("/ManageRoles/Index")]

        public async Task NonPublicPages_Should_Return_Unauthorized_When_Not_Authenticated(string URL)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });


            var response = await client.GetAsync(URL);
            var code = response.StatusCode;

            Assert.Equal(HttpStatusCode.Unauthorized, code);
        }

        [Theory]
        [InlineData("/Event/Create")]
        [InlineData("/Event/Edit")]
        [InlineData("/Event/Delete")]
        [InlineData("/Team/Index")]
        //[InlineData("/Team/Create")]
        //[InlineData("/Team/Edit")]
        [InlineData("/Race/Index")]
        [InlineData("/Race/Create")]
        [InlineData("/Race/Detail")]
        [InlineData("/Race/Edit")]
        [InlineData("/ManageRoles/Index")]

        public async Task AdminPages_Should_Return_Forbidden_When_Member(string URL)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,

            });
            client.DefaultRequestHeaders.Add("Test-Role", "Member");

            var response = await client.GetAsync(URL);
            var code = response.StatusCode;
            
            Assert.Equal(HttpStatusCode.Forbidden, code);
        }

        [Theory]
        [InlineData("/Event/Index")]
        [InlineData("/Team/TimerRaces")]
        [InlineData("/Team/Detail/1")]

        public async Task MemberPages_Should_Return_OK_When_Member(string URL)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            client.DefaultRequestHeaders.Add("Test-Role", "Member");

            var response = await client.GetAsync(URL);
            var code = response.StatusCode;

            Assert.Equal(HttpStatusCode.OK, code);
        }

        [Theory]
        [InlineData("/Event/Index")]
        [InlineData("/Event/Create")]
        [InlineData("/Team/TimerRaces")]
        [InlineData("/Team/Detail/1")]
        [InlineData("/Team/Index")]
        //[InlineData("/Team/Create")]
        //[InlineData("/Team/Edit")]
        [InlineData("/Race/Index")]
        [InlineData("/Race/Create")]
        [InlineData("/ManageRoles/Index")]

        public async Task AllPages_Should_Return_OK_When_Admin(string URL)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,

            });
            client.DefaultRequestHeaders.Add("Test-Role", "Member,Admin");

            var response = await client.GetAsync(URL);
            var code = response.StatusCode;

            Assert.Equal(HttpStatusCode.OK, code);
        }
    }
}