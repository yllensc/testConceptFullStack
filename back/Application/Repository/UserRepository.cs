using Domine.Data;
using Domine.Entities;
using Domine.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Repository;
public class UserRepository : GenericRepository<User>, IUser
{

    protected readonly TestFullStackDbContext _context;
    public UserRepository(TestFullStackDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User> GetUserName(string username)
    {
        return await _context.Users
                            .Include(u => u.Roles)
                            .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
    }

    public async Task<User> GetSomeUserLogic(string id)
    {
        var idInt = Int32.Parse(id);
        return await _context.Users
                            .Include(u => u.Roles)
                            .FirstOrDefaultAsync(u => u.Id == idInt);
    }

    public async Task<User> GetByRefreshTokenAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
    }


}