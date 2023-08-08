using Application.Users.Dtos;
using Core.Models;
using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IDataHandler<T>
{
    T Save(T data);
    DataResum<T> Get(UserFilter? userFilter);
    Task<bool> BulkUserDataFromFile(Stream stream)
    {
        return Task.FromResult(true);
    }
}
