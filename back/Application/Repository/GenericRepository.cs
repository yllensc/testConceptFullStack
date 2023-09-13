using System.Linq.Expressions;
using Domine.Data;
using Domine.Entities;
using Domine.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repository;
    public class GenericRepository<T> : IGeneric<T> where T : BaseEntity
{
    private readonly TestFullStackDbContext _context;

    public GenericRepository(TestFullStackDbContext context)
    {
        _context = context;
    }

    public virtual void Add(T entity)
    {
        
        _context.Set<T>().Add(entity);
    }

    public virtual void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression);
    }

    public virtual async Task<(int totalRecords, IEnumerable<T> records)> GetAllAsync(int pageIndex, int pageSize, string search){
        var totalRecords = await _context.Set<T>().CountAsync();
        var records = await _context.Set<T>()
            .Skip((pageIndex -1)*pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (totalRecords, records);
    }
 
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public virtual void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public virtual void Update(T entity)
    {
        _context.Set<T>()
            .Update(entity);
    }
}