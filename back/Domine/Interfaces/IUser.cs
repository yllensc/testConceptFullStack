using Domine.Entities;

namespace Domine.Interfaces
{
    public interface IUser : IGeneric<User>
    {
        Task<User>GetSomeUserLogic(string id);
        Task<User> GetUserName(string userName);
        Task<User> GetByRefreshTokenAsync(string username);
    }
}