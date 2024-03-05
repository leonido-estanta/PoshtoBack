using System.Linq.Expressions;

namespace PoshtoBack.Data;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly PoshtoDbContext Context;

    public Repository(PoshtoDbContext context)
    {
        Context = context;
    }

    public IQueryable<T> GetAll()
    {
        return Context.Set<T>();
    }

    public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return Context.Set<T>().Where(predicate);
    }

    public T GetById(int id)
    {
        return Context.Set<T>().Find(id);
    }

    public void Add(T entity)
    {
        Context.Set<T>().Add(entity);
    }

    public void Remove(T entity)
    {
        Context.Set<T>().Remove(entity);
    }

    public void Update(T entity)
    {
        Context.Set<T>().Update(entity);
    }
}