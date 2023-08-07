namespace Domain.Interfaces;

public interface ISaver<T>
{
    T Save(T data);
}
