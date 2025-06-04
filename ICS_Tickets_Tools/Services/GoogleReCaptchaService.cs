using ICS_Tickets_Tools.DB_Context;
using ICS_Tickets_Tools.Hubs;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace ICS_Tickets_Tools.Services
{
    public class GoogleReCaptchaService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleReCaptchaService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> VerifyToken(string token)
        {
            var secretKey = _configuration["RecaptchaSettings:SecretKey"];
            var url = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}";

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync(url);
            var captchaResponse = JsonSerializer.Deserialize<GoogleReCaptchaResponse>(response);

            return captchaResponse != null && captchaResponse.success && captchaResponse.score >= 0.5;
        }

        private class GoogleReCaptchaResponse
        {
            public bool success { get; set; }
            public float score { get; set; }
            public string action { get; set; }
            public string challenge_ts { get; set; }
            public string hostname { get; set; }
        }       
    }
}


