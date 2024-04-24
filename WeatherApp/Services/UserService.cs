using log4net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherApp.Context.DBSettings;
using WeatherApp.Context.Result;
using WeatherApp.Context.User;
using WeatherApp.Context.WeatherLocation;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class UserService : IUserService
    {
        private static ILog _log = LogManager.GetLogger(typeof(UserService));
        private readonly IMongoCollection<UserEntity> _user;
        private readonly HttpClient _httpClient;
        private readonly IHubContext<SignalrHub> _hubContext;
        private readonly JWTSettings _jwtSettings;
        public UserService(IOptions<MongoDBSettings> mongoDBSettings, IMongoDBService mongoDBService, IHubContext<SignalrHub> hubContext, IOptions<JWTSettings> jwtSettings)
        {
            IMongoDatabase weatherDatabase = mongoDBService.GetWeatherDatabase;
            _user = weatherDatabase.GetCollection<UserEntity>(mongoDBSettings.Value.UserCollectionName);
            _httpClient = new HttpClient();
            _hubContext = hubContext;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
        {
            var userResult = await GetUserByUsernameAndPassword(model.Username, model.Password);

            // return null if user not found
            if (!userResult.Status || userResult.Data == null)
            {
                return null;
            }

            // authentication successful so generate jwt token
            var token = await generateJwtToken(userResult.Data);

            return new AuthenticateResponse(userResult.Data, token);
        }
        public async Task<ResultEntity<UserEntity?>> GetUserByUsernameAndPassword(string username, string password)
        {
            try
            {
                var user = await _user.Find(user => user.Username == username && user.Password == password).FirstOrDefaultAsync();
                _log.Info($"GetAllWeatherLocations: locations {JsonConvert.SerializeObject(user)}");
                if (user == null)
                {
                    return new ResultEntity<UserEntity?>
                    {
                        Status = false,
                        Data = null,
                        ResultCode = ResultCode.Status404NotFound,
                        Message = "User not found"
                    };
                }
                return new ResultEntity<UserEntity?>
                {
                    Status = true,
                    Data = user,
                    ResultCode = ResultCode.Status200OK,
                    Message = "Weather locations are read successfully"
                };
            }
            catch (Exception ex)
            {
                _log.Error($"GetAllWeatherLocations: Error occurred while get locations", ex);
                return new ResultEntity<UserEntity?>
                {
                    Status = false,
                    Data = null,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
            }
        }
        public async Task<ResultEntity<UserEntity?>> GetUserById(string id)
        {
            try
            {
                var user = await _user.Find(user => user.Id == id).SortByDescending(user => user.CreatedOn).FirstOrDefaultAsync();
                _log.Info($"GetAllWeatherLocations: locations {JsonConvert.SerializeObject(user)}");
                if (user == null)
                {
                    return new ResultEntity<UserEntity?>
                    {
                        Status = true,
                        Data = null,
                        ResultCode = ResultCode.Status404NotFound,
                        Message = "User not found"
                    };
                }
                return new ResultEntity<UserEntity?>
                {
                    Status = true,
                    Data = user,
                    ResultCode = ResultCode.Status200OK,
                    Message = "Weather locations are read successfully"
                };
            }
            catch (Exception ex)
            {
                _log.Error($"GetAllWeatherLocations: Error occurred while get locations", ex);
                return new ResultEntity<UserEntity?>
                {
                    Status = false,
                    Data = null,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
            }
        }

        private async Task<string> generateJwtToken(UserEntity user)
        {
            //Generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {

                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);
        }
    }
}
