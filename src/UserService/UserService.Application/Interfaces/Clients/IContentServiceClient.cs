using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.Interfaces.Clients
{
    public interface IContentServiceClient
    {
        Task IncrementFavoritesAsync(Guid routeId);

        Task DecrementFavoritesAsync(Guid routeId);
    }
}
