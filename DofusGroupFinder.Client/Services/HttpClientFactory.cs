using System;
using System.Net.Http;

namespace DofusGroupFinder.Client.Services
{
    public static class HttpClientFactory
    {
        private static readonly Lazy<HttpClient> lazyClient = new Lazy<HttpClient>(() =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5209/") // à centraliser plus tard dans une config globale
            };
            return client;
        });

        public static HttpClient Instance => lazyClient.Value;
    }
}