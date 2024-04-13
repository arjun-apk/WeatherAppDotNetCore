using WeatherApp.Context.Result;

namespace WeatherApp.Context.WeatherLocation
{
    public interface IWeatherLocationService
    {
        Task<ResultEntity<List<WeatherLocationEntity>>> GetAllWeatherLocations();
        Task<ResultEntity<WeatherLocationEntity>> GetWeatherLocationById(string id);
        Task<ResultEntity<bool>> CreateWeatherLocation(string locationName);
        Task<ResultEntity<bool>> DeleteWeatherLocation(string id);
    }
}
