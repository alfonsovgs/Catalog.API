using Catalog.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            return result.Succeeded;

        }

        public async Task<bool> SignUpAsync(User user, string password, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<User> GetByEmailAsync(string requestEmail, CancellationToken cancellationToken = default)
        {
            return await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.Email == requestEmail, cancellationToken);
        }
    }
}
