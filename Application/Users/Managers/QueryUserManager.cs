using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Dtos;
using Core.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Users.Managers;

public class QueryUserManager : IQuerableFilterable<User, UserFilter>
{
    private readonly IDataHandler<User> _dataHandler;
    private readonly ICacheService _cacheService;

    public QueryUserManager(IDataHandler<User> dataHandler, ICacheService cacheService)
    {
        _dataHandler = dataHandler;
        _cacheService = cacheService;
    }

    public DataResum<User> Get(UserFilter? userFilter)
    {
        var cacheKey = userFilter?.ToString() ?? "QueryUserManager:Default";

        var resume = _cacheService.GetOrSave(
                    cacheKey,
                    () => Task.FromResult(_dataHandler.Get(userFilter))
               ).Result;

        return Pagination<User>.Create(
            resume,
            userFilter.PageIndex,
            userFilter.PageSize
        );
    }
}
