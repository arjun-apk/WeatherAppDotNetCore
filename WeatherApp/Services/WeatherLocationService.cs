using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WeatherApp.Context.DBSettings;
using WeatherApp.Context.Result;
using WeatherApp.Context.WeatherLocation;
using WeatherApp.Models;
using log4net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;

namespace WeatherApp.Services
{
    public class WeatherLocationService : IWeatherLocationService
    {
        private static ILog _log = LogManager.GetLogger(typeof(WeatherLocationService));
        private readonly IMongoCollection<WeatherLocationEntity> _weatherLocation;
        private readonly HttpClient _httpClient;
        private readonly IHubContext<SignalrHub> _hubContext;

        public WeatherLocationService(IOptions<MongoDBSettings> mongoDBSettings, IMongoDBService mongoDBService, IHubContext<SignalrHub> hubContext)
        {
            IMongoDatabase weatherDatabase = mongoDBService.GetWeatherDatabase;
            _weatherLocation = weatherDatabase.GetCollection<WeatherLocationEntity>(mongoDBSettings.Value.WeatherLocationCollectionName);
            _httpClient = new HttpClient();
            _hubContext = hubContext;
        }
        public async Task<ResultEntity<List<WeatherLocationEntity>>> GetAllWeatherLocations()
        {
            try
            {
                var locations = await _weatherLocation.Find(_ => true).SortByDescending(location => location.CreatedOn).ToListAsync();
                _log.Info($"GetAllWeatherLocations: locations {JsonConvert.SerializeObject(locations)}");
                if (locations == null)
                {
                    return new ResultEntity<List<WeatherLocationEntity>>
                    {
                        Status = true,
                        Data = [],
                        ResultCode = ResultCode.Status200OK,
                        Message = "Weather locations is empty"
                    };
                }
                return new ResultEntity<List<WeatherLocationEntity>>
                {
                    Status = true,
                    Data = locations,
                    ResultCode = ResultCode.Status200OK,
                    Message = "Weather locations are read successfully"
                };
            }
            catch (Exception ex)
            {
                _log.Error($"GetAllWeatherLocations: Error occurred while get locations", ex);
                return new ResultEntity<List<WeatherLocationEntity>>
                {
                    Status = false,
                    Data = null,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
            }
        }

        public async Task<ResultEntity<WeatherLocationEntity>> GetWeatherLocationById(string id)
        {
            try
            {
                _log.Info($"GetWeatherLocations: weather location id {id}");
                var location = await _weatherLocation.Find(location => location.Id == id).FirstOrDefaultAsync();
                _log.Info($"GetWeatherLocations: location {JsonConvert.SerializeObject(location)}");
                if (location == null)
                {
                    return new ResultEntity<WeatherLocationEntity>
                    {
                        Status = false,
                        Data = null,
                        ResultCode = ResultCode.Status404NotFound,
                        Message = "Weather location is not found"
                    };
                }
                return new ResultEntity<WeatherLocationEntity>
                {
                    Status = true,
                    Data = location,
                    ResultCode = ResultCode.Status200OK,
                    Message = "Weather location read successfully"
                };
            }
            catch (Exception ex)
            {
                _log.Error($"GetWeatherLocations: Error occurred while get location by weather location id, id {id}", ex);
                return new ResultEntity<WeatherLocationEntity>
                {
                    Status = false,
                    Data = null,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
            }
        }

        public async Task<ResultEntity<bool>> CreateWeatherLocation(string locationName)
        {
            try
            {
                _log.Info($"CreateWeatherLocation: weatherLocation {locationName}");
                string apiKey = "cb7e4b494e5e4949b4b125956240104";
                string currentWeatherApiUrl = $"https://api.weatherapi.com/v1/current.json?q={locationName}&key={apiKey}";
                using (HttpClient client = new HttpClient())
                {
                    var currentWeatherApiResponse = await _httpClient.GetAsync(currentWeatherApiUrl);
                    if (currentWeatherApiResponse.IsSuccessStatusCode)
                    {
                        var currentWeatherApiResult = await currentWeatherApiResponse.Content.ReadAsStringAsync();
                        var currentWeatherApiData = JsonConvert.DeserializeObject<WeatherLocation>(currentWeatherApiResult);
                        if (currentWeatherApiData != null)
                        {
                            var weatherLocation = new WeatherLocationEntity
                            {
                                Location = currentWeatherApiData.Location.Name,
                                Region = currentWeatherApiData.Location.Region,
                                Country = currentWeatherApiData.Location.Country,
                                Lat = currentWeatherApiData.Location.Lat,
                                Lon = currentWeatherApiData.Location.Lon,
                                LocalTime = currentWeatherApiData.Location.Localtime,
                                LastUpdated = currentWeatherApiData.Current.Last_updated,
                                TempC = currentWeatherApiData.Current.Temp_c,
                                TempF = currentWeatherApiData.Current.Temp_f,
                                IsDay = currentWeatherApiData.Current.Is_day,
                                ConditionText = currentWeatherApiData.Current.Condition.Text,
                                ConditionIcon = currentWeatherApiData.Current.Condition.Icon,
                                ConditionCode = currentWeatherApiData.Current.Condition.Code,
                                CreatedOn = DateTime.UtcNow
                            };
                            await _weatherLocation.InsertOneAsync(weatherLocation);
                            var result = await GetAllWeatherLocations();
                            if (result.Status)
                            {
                                await _hubContext.Clients.All.SendAsync("WeatherLocation", result.Data);
                            }
                            return new ResultEntity<bool>
                            {
                                Status = true,
                                Data = true,
                                ResultCode = ResultCode.Status201Created,
                                Message = "Weather location created successfully"
                            };
                        }
                        else
                        {
                            return new ResultEntity<bool>
                            {
                                Status = false,
                                Data = false,
                                ResultCode = ResultCode.Status400BadRequest,
                                Message = "Weather location not found"
                            };
                        }
                    }
                    else
                    {
                        return new ResultEntity<bool>
                        {
                            Status = false,
                            Data = false,
                            ResultCode = ResultCode.Status400BadRequest,
                            Message = "Invalid location"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"CreateWeatherLocation: Error occurred while create weather location, weatherLocation {locationName}", ex);
                return new ResultEntity<bool>
                {
                    Status = false,
                    Data = false,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
            }
        }

        public async Task<ResultEntity<bool>> DeleteWeatherLocation(string id)
        {
            try
            {
                _log.Info($"DeleteWeatherLocation: weather location id {id}");
                var result = await _weatherLocation.DeleteOneAsync(location => location.Id == id);
                _log.Info($"DeleteWeatherLocation: result {JsonConvert.SerializeObject(result)}");
                if (result.DeletedCount <= 0)
                {
                    return new ResultEntity<bool>
                    {
                        Status = false,
                        Data = false,
                        ResultCode = ResultCode.Status404NotFound,
                        Message = "Weather location is not found"
                    };
                }
                var weatherLocations = await GetAllWeatherLocations();
                if (weatherLocations.Status)
                {
                    await _hubContext.Clients.All.SendAsync("WeatherLocation", weatherLocations.Data);
                }
                return new ResultEntity<bool>
                {
                    Status = true,
                    Data = true,
                    ResultCode = ResultCode.Status200OK,
                    Message = "Weather locations deleted successfully"
                };

            }
            catch (Exception ex)
            {
                _log.Error($"DeleteWeatherLocation: Error occurred while get locations", ex);
                return new ResultEntity<bool>
                {
                    Status = false,
                    Data = false,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
            }
        }
    }
}
