namespace Chat.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<List<T>> FindAllAsync();
    Task<T> CreateAsync(T customer);
    Task<T> UpdateAsync(T customer);
    Task DeleteAsync(int id);
    Task<T?> FindByIdAsync(int id);
}
