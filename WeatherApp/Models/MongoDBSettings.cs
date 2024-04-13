namespace WeatherApp.Models
{
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string WeatherLocationDatabaseName { get; set; } = null!;
        public string WeatherLocationCollectionName { get; set; } = null!;
    }
}
