namespace Application.Common.Interfaces;

public interface IAuthService
{
    Task<bool> ValidateCredentials(string username, string password);
}
