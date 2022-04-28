using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Weather.Models;
//Requires nuget package System.Net.Http.Json

namespace Weather.Services
{
    public class OpenWeatherService
    {
        private DateTime _timeStampNow;
        private readonly HttpClient _httpClient = new HttpClient();
        public EventHandler<string> WeatherForecastAvailable;
        readonly string apiKey = "0e78426112ba490f9844a60dbdcd7963"; // Your API Key
        private readonly ConcurrentDictionary<(DateTime, string), Forecast> _cachedForecast = new ConcurrentDictionary<(DateTime, string), Forecast>();

        public async Task<Forecast> GetForecastAsync(string city)
        {
            _timeStampNow = DateTime.Now;
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&units=metric&lang={language}&appid={apiKey}";

            foreach (var item in _cachedForecast)
            {
                if (item.Key.Item2.ToLower() == city.ToLower())
                {
                    if (item.Key.Item1.AddMinutes(1) > _timeStampNow)
                    {
                        WeatherForecastAvailable?.Invoke(this, $"Cached weather forecast for {city} available");
                        return item.Value;
                    }
                    if(!_cachedForecast.TryRemove(item.Key, out _))
                        Console.WriteLine("Couldn't remove item from cache.");
                }
            }
            
            var forecast = await ReadWebApiAsync(uri);
            if (!_cachedForecast.TryAdd((_timeStampNow, city), forecast))
                Console.WriteLine("Couldn't add item to cache.");

            
            WeatherForecastAvailable?.Invoke(this, $"New weather forecast for {city} available");
            
            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            // part of your read web api code here
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            // part of your data transformation to Forecast here
            var forecast = new Forecast()
            {
                City = wd?.city.name,
                Items = new List<ForecastItem>()
            };

            wd?.list.ForEach(x =>
                forecast.Items.Add(new ForecastItem()
                {
                    DateTime = UnixTimeStampToDateTime(x.dt),
                    Temperature = x.main.temp,
                    WindSpeed = x.wind.speed,
                    Description = x.weather.First().description,
                    Icon = x.weather.First().icon
                }));

            return forecast;
        }
        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
