using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces.Clients;

namespace UserService.Infrastructure.Clients
{
    public class ContentServiceClient : IContentServiceClient
    {
        private readonly HttpClient _httpClient;

        public ContentServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task IncrementFavoritesAsync(Guid routeId)
        {
            var response = await _httpClient.PostAsync(
                $"api/route-stats/{routeId}/favorite/increment",
                null);
            response.EnsureSuccessStatusCode();
        }

        public async Task DecrementFavoritesAsync(Guid routeId)
        {
            var response = await _httpClient.PostAsync(
                $"api/route-stats/{routeId}/favorite/decrement",
                null);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return;
            }

            response.EnsureSuccessStatusCode();
        }
    }
}
