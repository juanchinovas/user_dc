namespace Domain.Interfaces;

public interface IFileProcessor
{
    Task<bool> Process(Stream stream);
}
