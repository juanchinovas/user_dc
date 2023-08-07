using Domain.Entities;

namespace Domain.Interfaces;

public interface IQuerableFiltrable<T, F> where F : notnull
{
    Tuple<int, IEnumerable<T>> Get(F? filter);
}
