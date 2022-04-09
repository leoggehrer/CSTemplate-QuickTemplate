//@BaseCode
//MdStart

namespace QuickTemplate.Logic
{
    public interface IDataAccess<T> : IDisposable
    {
        Task<T[]> GetAllAsync();
        ValueTask<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities);
        Task<T> InsertAsync(T entity);

        Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> entities);
        Task<T> UpdateAsync(T entity);

        Task DeleteAsync(int id);

        Task<int> SaveChangesAsync();
    }
}
//MdEnd