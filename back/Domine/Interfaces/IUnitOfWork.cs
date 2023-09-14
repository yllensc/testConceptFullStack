namespace Domine.Interfaces
{
    public interface IUnitOfWork
    {
        IUser Users { get; }
        IRol Roles {get;}
        IClient Clients {get; }
        IHistorialTokens HistorialTokens {get; }
        Task<int> SaveAsync();
    }
}