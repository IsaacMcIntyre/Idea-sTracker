using System;
using IdeasTracker.Business.Converters.Interfaces;
using IdeasTracker.Database.Entities;
using IdeasTracker.Models;

namespace IdeasTracker.Business.Converters
{
    public class UserToUserModelConverter : IUserToUserModelConverter
    {
        public UserModel Convert(User user)
        {
            if (user == null)
                return null;

            return new UserModel
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                Role = user.Role
            };
        }
    }
}
