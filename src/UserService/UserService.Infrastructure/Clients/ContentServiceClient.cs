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
            await _httpClient.PostAsync(
                $"api/route-stats/{routeId}/favorite/increment",
                null);
        }

        public async Task DecrementFavoritesAsync(Guid routeId)
        {
            await _httpClient.PostAsync(
                $"api/route-stats/{routeId}/favorite/decrement",
                null);
        }
    }
}
