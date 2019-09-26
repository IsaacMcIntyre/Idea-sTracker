using System.Collections.Generic;
using System.Threading.Tasks;
using IdeasTracker.Models;

namespace IdeasTracker.Business.Uows.Interfaces
{
    public interface IUserUow
    {
        Task<List<UserModel>> GetUsersAsync();
        Task CreateUserAsync(UserModel userModel);
        Task<UserModel> GetUserAsync(int? id);
        Task DeleteUserAsync(int? id);
        Task EditUserAsync(UserModel userModel);
        Task<bool> IsUserExistsAsync(int id);
    }
}
