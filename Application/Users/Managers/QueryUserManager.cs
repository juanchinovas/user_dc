using Application.Common.Interfaces;
using Application.Users.Dtos;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Users.Managers;

public class QueryUserManager : IQuerableFiltrable<User, UserFilter>
{
    private readonly IDataHandler<User> _dataHandler;

    public QueryUserManager(IDataHandler<User> dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public Tuple<int, IEnumerable<User>> Get(UserFilter? userFilter)
    {
        return _dataHandler.Get(userFilter);
    }
}
