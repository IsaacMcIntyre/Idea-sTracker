using System.Collections.Generic;
using System.Threading.Tasks;
using IdeasTracker.Business.Converters.Interfaces;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Database.Context;
using IdeasTracker.Database.Entities;
using IdeasTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace IdeasTracker.Business.Uow
{
    public class UserUow: IUserUow
    {
        private readonly ApplicationDbContext _context;
        public readonly IUserToUserModelConverter _userToUserModelConverter;


        public UserUow(ApplicationDbContext context, IUserToUserModelConverter userToUserModelConverter)
        {
            _context = context;
            _userToUserModelConverter = userToUserModelConverter;
        }
        public async Task<List<UserModel>> GetUsersAsync()
        {
            var users = new List<UserModel>();
            await _context.Users.ForEachAsync(user =>
            {
                users.Add(new UserModel
                {
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.Name,
                    Role = user.Role
                });
            });
            return users;
        }
        public async Task CreateUserAsync(UserModel userModel)
        {
            _context.Add(new User
            {
                Email = userModel.Email,
                Name = userModel.Name,
                Role = userModel.Role
            });
            await _context.SaveChangesAsync();
        }

        public async Task<UserModel> GetUserAsync(int? id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            return _userToUserModelConverter.Convert(user);
        }

        public async Task DeleteUserAsync(int? id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
