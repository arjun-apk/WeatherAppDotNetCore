namespace WeatherApp.Context.Result
{
    public class ResultEntity<T>
    {
        public required bool Status { get; set; }
        public required string Message { get; set; }
        public required T Data { get; set; }
        public required int ResultCode { get; set; }
    }

    public class ResultCode
    {
        public const int Status200OK = 200;
        public const int Status201Created = 201;
        public const int Status204NoContent = 204;
        public const int Status304NotModified = 304;
        public const int Status400BadRequest = 400;
        public const int Status401Unauthorized = 401;
        public const int Status404NotFound = 404;
        public const int Status500InternalServerError = 500;
    }
}
