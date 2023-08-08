using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Dtos;
using Core.Models;
using Domain.Entities;

namespace Infrastructure.Data.Memory;

public class MemoryDataHandler : IDataHandler<User>
{
    private IEnumerable<User> _users;

    public MemoryDataHandler()
    {
        _users = Array.Empty<User>();
    }

    public DataResum<User> Get(UserFilter? userFilter = null)
    {
        if (userFilter == null)
        {
            var totalItems = _users.Count();
            return new DataResum<User>
            {
                Items = _users,
                TotalItems = totalItems
            };
        }

        var filteredUsers = _users
            .Where(u => !userFilter.Age.HasValue || u.Age == userFilter.Age)
            .Where(u => string.IsNullOrEmpty(userFilter.Country) || u.Country.Equals(userFilter.Country))
            .Skip((userFilter.PageIndex - 1) * userFilter.PageSize).Take(userFilter.PageSize);

        return new DataResum<User>
        {
            Items = filteredUsers,
            TotalItems = filteredUsers.Count()
        };
    }

    public User Save(User data)
    {
        data.Id = _users.Count() + 1;
        _users = _users.Append(data);

        return data;
    }
}
