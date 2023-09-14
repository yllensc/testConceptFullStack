using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Repository;
using Domine.Data;
using Domine.Interfaces;

namespace Application.UnitOfWork;
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly TestFullStackDbContext context;
    private RolRepository _roles;
    private UserRepository _users;
    private ClientRepository _clients;
    private HistorialTokensRepository _historial;

    public UnitOfWork(TestFullStackDbContext _context)
    {
        context = _context;
    }

    public IUser Users
    {
        get
        {
            _users ??= new UserRepository(this.context);
            return _users;
        }
    }

    public IClient Clients
    {
        get
        {
            _clients ??= new ClientRepository(this.context);
            return _clients;
        }
    }

    public IHistorialTokens HistorialTokens
    {
        get
        {
            _historial ??= new HistorialTokensRepository(this.context);
            return _historial;
        }
    }


    public IRol Roles
    {
        get
        {
            _roles ??= new RolRepository(this.context);
            return _roles;
        }
    }

    public async Task<int> SaveAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}

   

