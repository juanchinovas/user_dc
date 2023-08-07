using Application.Users.Dtos;
using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IDataHandler<T>
{
    T Save(T data);
    Tuple<int, IEnumerable<User>> Get(UserFilter? userFilter);
    Task<bool> BulkUserDataFromFile(Stream stream)
    {
        return Task.FromResult(true);
    }
}
