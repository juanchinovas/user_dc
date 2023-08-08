namespace Application.Common.Interfaces;

public interface ICacheService
{
    Task Save(string key, object value);
    Task<object?> Get(string key);
    Task<T> GetOrSave<T>(string key, Func<Task<T>> producer);
}
