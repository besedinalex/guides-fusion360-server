using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GuidesFusion360Server.Data
{
    /// <inheritdoc />
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext _context;

        public UsersRepository(DataContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<int> AddUser(UserModel user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        /// <inheritdoc />
        public Task<UserModel> GetUser(int userId) =>
            _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        /// <inheritdoc />
        public Task<UserModel> GetUser(string email) =>
            _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));

        public Task<List<UserModel>> GetUsers() =>
            _context.Users.ToListAsync();

        /// <inheritdoc />
        public Task<bool> UserExists(string email) =>
            _context.Users.AnyAsync(x => x.Email.ToLower().Equals(email.ToLower()));

        /// <inheritdoc />
        public async Task<bool> UserIsEditor(int userId)
        {
            var user = await GetUser(userId);
            return user != null && (user.Access == "editor" || user.Access == "admin");
        }

        /// <inheritdoc />
        public async Task<bool> UserIsAdmin(int userId)
        {
            var user = await GetUser(userId);
            return user != null && user.Access == "admin";
        }

        /// <inheritdoc />
        public Task UpdateUser(UserModel user)
        {
            _context.Users.Update(user);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task RemoveUser(UserModel user)
        {
            _context.Users.Remove(user);
            return _context.SaveChangesAsync();
        }
    }
}