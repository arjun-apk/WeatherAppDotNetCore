using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WeatherApp.Context.Result;
using WeatherApp.Context.WeatherLocation;

namespace WeatherApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherLocationController : ControllerBase
    {
        private static ILog _log = LogManager.GetLogger(typeof(WeatherLocationController));
        private readonly IWeatherLocationService _weatherLocationService;

        public WeatherLocationController(IWeatherLocationService weatherLocationService)
        {
            _weatherLocationService = weatherLocationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWeatherLocations()
        {
            try
            {
                _log.Info("GetAllWeatherLocations request started");
                var result = await _weatherLocationService.GetAllWeatherLocations();
                _log.Info($"GetAllWeatherLocations: result {JsonConvert.SerializeObject(result)}");
                _log.Info("GetAllWeatherLocations request end");
                return StatusCode(result.ResultCode, result);
            }
            catch (Exception ex)
            {
                _log.Error($"GetAllWeatherLocations: Exception occurred while get weather locations", ex);
                var result = new ResultEntity<bool>
                {
                    Status = false,
                    Data = false,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
                _log.Info("GetAllWeatherLocations request end");
                return StatusCode(result.ResultCode, result);
            }
        }

        [HttpGet, Route("/:id")]
        public async Task<IActionResult> GetWeatherLocationById(string id)
        {
            try
            {
                _log.Info("GetWeatherLocationById request started");
                _log.Info($"GetWeatherLocationById: weather location id {id}");
                var result = await _weatherLocationService.GetWeatherLocationById(id);
                _log.Info($"GetWeatherLocationById: result {JsonConvert.SerializeObject(result)}");
                _log.Info("GetWeatherLocationById request end");
                return StatusCode(result.ResultCode, result);
            }
            catch (Exception ex)
            {
                _log.Error($"GetWeatherLocationById: Exception occurred while get weather location by id, id {id}", ex);
                var result = new ResultEntity<bool>
                {
                    Status = false,
                    Data = false,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
                _log.Info("GetWeatherLocationById request end");
                return StatusCode(result.ResultCode, result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWeatherLocation(string locationName)
        {
            try
            {
                _log.Info("CreateWeatherLocation request started");
                _log.Info($"CreateWeatherLocation: weatherLocation {locationName}");
                var result = await _weatherLocationService.CreateWeatherLocation(locationName);
                _log.Info($"CreateWeatherLocation: result {JsonConvert.SerializeObject(result)}");
                _log.Info("CreateWeatherLocation request end");
                return StatusCode(result.ResultCode, result);
            }
            catch (Exception ex)
            {
                _log.Error($"CreateWeatherLocation: Exception occurred while create weather location, weatherLocation {locationName}", ex);
                var result = new ResultEntity<bool>
                {
                    Status = false,
                    Data = false,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
                _log.Info("CreateWeatherLocation request end");
                return StatusCode(result.ResultCode, result);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteWeatherLocationById(string id)
        {
            try
            {
                _log.Info("DeleteWeatherLocationById request started");
                _log.Info($"DeleteWeatherLocationById: weather location id {id}");
                var result = await _weatherLocationService.DeleteWeatherLocation(id);
                _log.Info($"DeleteWeatherLocationById: result {JsonConvert.SerializeObject(result)}");
                _log.Info("DeleteWeatherLocationById request end");
                return StatusCode(result.ResultCode, result);
            }
            catch (Exception ex)
            {
                _log.Error($"DeleteWeatherLocationById: Exception occurred while delete weather location by id, id {id}", ex);
                var result = new ResultEntity<bool>
                {
                    Status = false,
                    Data = false,
                    ResultCode = ResultCode.Status500InternalServerError,
                    Message = "Internal Server Error"
                };
                _log.Info("DeleteWeatherLocationById request end");
                return StatusCode(result.ResultCode, result);
            }
        }
    }
}
