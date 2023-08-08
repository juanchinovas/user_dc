using Core.Models;

namespace Domain.Interfaces;

public interface IQuerableFilterable<T, F> where F : notnull
{
    DataResum<T> Get(F? filter);
}
