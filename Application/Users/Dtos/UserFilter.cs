namespace Application.Users.Dtos;

public record UserFilter
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Age { get; set; }
    public DateTime? Date { get; set; }
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public override string ToString()
    {
        var parameters = new[]
        {
            FirstName ?? "",
            LastName ?? "",
            Age.HasValue ? Age.ToString() : "",
            Date.HasValue ? Date?.ToString("s") : "",
            Country ?? "",
            Province ?? "",
            City ?? ""
        }.Where(p => !string.IsNullOrEmpty(p)).ToArray();

        var key = string.Join('_', parameters);
        if (key == string.Empty)
        {
            return $"pagination:{PageIndex}_{PageSize}";
        }

        return $"userfilter:{key}";
    }
}