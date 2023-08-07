using Application.Common.Interfaces;

namespace Api.Services
{
    public class AuthService : IAuthService
    {
        public Task<bool> ValidateCredentials(string username, string password)
        {
            return Task.Run(() =>
            {
                return Environment.GetEnvironmentVariable("username") == username &&
                Environment.GetEnvironmentVariable("password") == password;
            });
        }
    }
}
