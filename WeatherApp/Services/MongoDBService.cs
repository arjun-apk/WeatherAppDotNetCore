using WeatherApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WeatherApp.Context.DBSettings;

namespace MongoExample.Services;

public class MongoDBService : IMongoDBService
{
    private readonly IMongoDatabase _weatherDatabase;
    private readonly IMongoDatabase _userDatabase;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        _weatherDatabase = client.GetDatabase(mongoDBSettings.Value.WeatherLocationDatabaseName);
        _userDatabase = client.GetDatabase(mongoDBSettings.Value.UserCollectionName);
    }

    IMongoDatabase IMongoDBService.GetWeatherDatabase =>  _weatherDatabase;
    IMongoDatabase IMongoDBService.GetUserDatabase =>  _userDatabase;
}