using System.Linq.Expressions;

namespace PoshtoBack.Data;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    IQueryable<T> Find(Expression<Func<T, bool>> predicate);
    T? GetById(int id);
    void Add(T entity);
    void Remove(T entity);
    void Update(T entity);
}