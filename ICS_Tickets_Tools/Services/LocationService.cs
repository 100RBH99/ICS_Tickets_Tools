using ICS_Tickets_Tools.DB_Context;
using ICS_Tickets_Tools.Hubs;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace ICS_Tickets_Tools.Services
{
    public class LocationService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public LocationService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetClientPublicIpAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync("https://api.ipify.org");
            return response;
        }

        public async Task<string> GetLocationAsync(string ipAddress)
        {
            try
            {
                var apiKey = _configuration["LocationService:ApiKey"];
                var url = $"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}&ip={ipAddress}";

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errContent = await response.Content.ReadAsStringAsync();
                    return $"Error: {response.StatusCode} - {errContent}";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<LocationResponse>(responseContent);

                return $"{data.city}, {data.state_prov}, {data.country_name}";

            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        private class LocationResponse
        {
            public string city { get; set; }
            public string state_prov { get; set; }
            public string country_name { get; set; }
        }

        //public async Task<string> GetLocationFromIpAsync(string ip)
        //{
        //    var url = $"https://ipapi.co/{ip}/json/";
        //    var client = _httpClientFactory.CreateClient();
        //    var json = await client.GetStringAsync(url);
        //    var location = JsonSerializer.Deserialize<LocationResponse>(json);
        //    return $"{location?.city}, {location?.region}, {location?.country_name}";
        //}

        //private class LocationResponse
        //{
        //    public string city { get; set; }
        //    public string region { get; set; }
        //    public string country_name { get; set; }
        //}



    }
}
