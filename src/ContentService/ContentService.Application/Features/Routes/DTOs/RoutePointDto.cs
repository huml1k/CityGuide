namespace ContentService.Application.Features.Routes.DTOs
{
    public class RoutePointDto
    {
        public Guid Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int OrderIndex { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }
    }
}
