using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetRoutes
{
    public class GetRoutesResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public int FavoritesCount { get; set; }

        public string? CoverImageExtension { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }= [];

    }
}
