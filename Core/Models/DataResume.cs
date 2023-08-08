namespace Core.Models;

public class DataResum<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
}
