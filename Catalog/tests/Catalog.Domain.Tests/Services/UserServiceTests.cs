using Catalog.Domain.Configurations;
using Catalog.Domain.Services;
using Catalog.Fixtures;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Catalog.Domain.Requests.User;
using Shouldly;
using Xunit;

namespace Catalog.Domain.Tests.Services
{
    public class UserServiceTests : IClassFixture<UsersContextFactory>
    {
        private readonly UserService _userService;

        public UserServiceTests(UsersContextFactory usersContextFactory)
        {
            _userService = new UserService(usersContextFactory.InMemoryUserManager, 
                Options.Create(new AuthenticationSettings
                {
                    Secret = "Very Secret key-word to match",
                    ExpirationDays = 7
                }));
        }

        [Fact]
        public async Task singin_with_invalid_user_should_return_a_valid_token_response()
        {
            var result = await _userService.SignInAsync(new SignInRequest
            {
                Email = "invalid.user",
                Password = "invalid_password"
            });
            result.ShouldBeNull();
        }

        [Fact]
        public async Task signin_with_valid_user_should_return_a_valid_token_response()
        {
            var result = await _userService.SignInAsync(new SignInRequest
            {
                Email = "alfonso@example.com",
                Password = "P@$$w0rd"
            });
            result.Token.ShouldNotBeEmpty();
        }
    }
}
