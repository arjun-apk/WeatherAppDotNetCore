using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WeatherApp.Context.WeatherLocation
{
    [BsonIgnoreExtraElements]
    public class WeatherLocationEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("location")]
        public string Location { get; set; } = string.Empty;

        [BsonElement("region")]
        public string Region { get; set; } = string.Empty;

        [BsonElement("country")]
        public string Country { get; set; } = string.Empty;

        [BsonElement("lat")]
        public double Lat { get; set; }

        [BsonElement("lon")]
        public double Lon { get; set; }

        [BsonElement("localTime")]
        public DateTime LocalTime { get; set; }

        [BsonElement("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [BsonElement("tempC")]
        public double TempC { get; set; }

        [BsonElement("tempF")]
        public double TempF { get; set; }

        [BsonElement("isDay")]
        public int IsDay { get; set; }

        [BsonElement("conditionText")]
        public string ConditionText { get; set; } = string.Empty;

        [BsonElement("conditionIcon")]
        public string ConditionIcon { get; set; } = string.Empty;

        [BsonElement("conditionCode")]
        public int ConditionCode { get; set; }

        [BsonElement("createdOn")]
        public DateTime CreatedOn { get; set; }
    }
}

