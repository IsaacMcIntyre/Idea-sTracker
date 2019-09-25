using System;
using IdeasTracker.Database.Entities;
using IdeasTracker.Models;

namespace IdeasTracker.Business.Converters.Interfaces
{
    public interface IUserToUserModelConverter
    {
        UserModel Convert(User user);
    }
}
