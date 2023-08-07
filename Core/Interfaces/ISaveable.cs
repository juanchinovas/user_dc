namespace Domain.Interfaces;

public interface ISaveable<T>
{
    T Save(T data);
}
