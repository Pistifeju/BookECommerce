using System.Linq.Expressions;
using BookECommerce.DataAccess.Data;
using BookECommerce.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BookECommerce.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    internal DbSet<T> _dbSet;
    
    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
        _dbContext.Products.Include(u => u.Category);
    }
    
    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProperty in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }
        return query.ToList();
    }

    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracking = false)
    {
        if (!tracking)
        {
            _dbSet.AsNoTracking();
        }
        
        IQueryable<T> query = _dbSet;
        query = query.Where(filter);
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProperty in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }
        return query.FirstOrDefault();
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    
    public void RemoveRange(IEnumerable<T> entity)
    {
        _dbSet.RemoveRange(entity);
    }

    public void DetachEntity(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Detached;
    }
}