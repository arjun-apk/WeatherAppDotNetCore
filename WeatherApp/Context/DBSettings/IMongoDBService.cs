using MongoDB.Driver;

namespace WeatherApp.Context.DBSettings
{
    public interface IMongoDBService
    {
        public IMongoDatabase GetWeatherDatabase { get; }
        public IMongoDatabase GetUserDatabase { get; }
    }
}
