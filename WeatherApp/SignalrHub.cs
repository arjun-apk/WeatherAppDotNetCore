using Microsoft.AspNetCore.SignalR;
using WeatherApp.Context.WeatherLocation;

namespace WeatherApp
{
    public class SignalrHub : Hub
    {
        public async Task WeatherLocation(List<WeatherLocationEntity> data)
        {
            await Clients.All.SendAsync("WeatherLocation", data);
        }
    }
}
