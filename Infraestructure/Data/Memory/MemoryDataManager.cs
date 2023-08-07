using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Dtos;
using Domain.Entities;

namespace Infrastructure.Data.Memory;

public class MemoryDataManager : IDataHandler<User>
{
    private IEnumerable<User> _users;

    public MemoryDataManager()
    {
        _users = Array.Empty<User>();
    }

    public Tuple<int, IEnumerable<User>> Get(UserFilter? userFilter = null)
    {
        if (userFilter == null)
        {
            var totalItems = _users.Count();
            return new Tuple<int, IEnumerable<User>>(totalItems, _users);
        }

        var filteredUsers = _users
            .Where(u => !userFilter.Age.HasValue || u.Age == userFilter.Age)
            .Where(u => string.IsNullOrEmpty(userFilter.Country) || u.Country.Equals(userFilter.Country))
            .Skip((userFilter.PageIndex - 1) * userFilter.PageSize).Take(userFilter.PageSize);

        return new Tuple<int, IEnumerable<User>>(filteredUsers.Count(), filteredUsers);
    }

    public User Save(User data)
    {
        data.Id = _users.Count() + 1;
        _users = _users.Append(data);

        return data;
    }
}
