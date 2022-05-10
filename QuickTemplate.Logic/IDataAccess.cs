//@BaseCode
//MdStart

namespace QuickTemplate.Logic
{
    /// <summary>
    /// Generic interface for data access.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    public partial interface IDataAccess<T> : IDisposable
    {
        /// <summary>
        /// Gets the number of quantity in the collection.
        /// </summary>
        /// <returns>Number of elements in the collection.</returns>
        Task<int> CountAsync();
        /// <summary>
        /// Returns the number of quantity in the collection based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Number of entities in the collection.</returns>
        Task<int> CountAsync(string predicate, params string[] includeItems);

        /// <summary>
        /// Returns all interfaces of the elements in the collection.
        /// </summary>
        /// <returns>All interfaces of the element collection.</returns>
        Task<T[]> GetAllAsync();
        /// <summary>
        /// Returns all interfaces of the elements in the collection.
        /// </summary>
        /// <param name="includeItems">The include items</param>
        /// <returns>All interfaces of the element collection (with include).</returns>
        Task<T[]> GetAllAsync(params string[] includeItems);
        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        Task<T[]> QueryAsync(string predicate, params string[] includeItems);

        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        ValueTask<T?> GetByIdAsync(int id);
        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The element of the type T with the corresponding identification (with includes).</returns>
        Task<T?> GetByIdAsync(int id, params string[] includeItems);

        /// <summary>
        /// The element is being tracked by the context but does not yet exist in the repository. 
        /// </summary>
        /// <param name="element">The element which is to be inserted.</param>
        /// <returns>The inserted element.</returns>
        Task<T> InsertAsync(T element);
        /// <summary>
        /// The elements are being tracked by the context but does not yet exist in the repository. 
        /// </summary>
        /// <param name="elements">The elements which are to be inserted.</param>
        /// <returns>The inserted elements.</returns>
        Task<IEnumerable<T>> InsertAsync(IEnumerable<T> elements);

        /// <summary>
        /// The element is being tracked by the context and exists in the repository, and some or all of its property values have been modified.
        /// </summary>
        /// <param name="element">The element which is to be updated.</param>
        /// <returns>The the modified element.</returns>
        Task<T> UpdateAsync(T element);
        /// <summary>
        /// The elements are being tracked by the context and exists in the repository, and some or all of its property values have been modified.
        /// </summary>
        /// <param name="elements">The elements which are to be updated.</param>
        /// <returns>The updated elements.</returns>
        Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> elements);

        /// <summary>
        /// Removes the element from the repository with the appropriate idelement.
        /// </summary>
        /// <param name="id">The identification.</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Saves any changes in the underlying persistence.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        Task<int> SaveChangesAsync();
    }
}
//MdEnd