using ElasticEmail.Model;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static ElasticEmailClient.Api;
using System.Diagnostics;
using System.Security.Policy;
using System;
using EmailSend = ElasticEmail.Model.EmailSend;
using Newtonsoft.Json;
using System.Text;

namespace Gamificacion_API.Models
{
    public class ElasticEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration; // Asegúrate de que la configuración está definida aquí

        public ElasticEmailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration; // y aquí se asigna a la variable de la clase
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var postData = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("subject", subject),
            new KeyValuePair<string, string>("from", _configuration["ElasticEmail:SenderEmail"]), // Nota el uso de _configuration
            new KeyValuePair<string, string>("to", to),
            new KeyValuePair<string, string>("bodyHtml", htmlContent),
            new KeyValuePair<string, string>("apikey", _configuration["ElasticEmail:ApiKey"]) // Nota el uso de _configuration
        };

            var content = new FormUrlEncodedContent(postData);
            var response = await _httpClient.PostAsync("https://api.elasticemail.com/v2/email/send", content);
            response.EnsureSuccessStatusCode();
        }
    }
}



