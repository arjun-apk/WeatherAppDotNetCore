using WeatherApp.Context.Result;
using WeatherApp.Models;

namespace WeatherApp.Context.User
{
    public interface IUserService
    {
        Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
        Task<ResultEntity<UserEntity?>> GetUserById(string id);
    }
}
