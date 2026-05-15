namespace ContentService.Application.Features.Routes.DTOs
{
    public class RouteImageDto
    {
        public Guid Id { get; set; }

        public string FileExtension { get; set; } = string.Empty;

        public bool IsCover { get; set; }

        public int OrderIndex { get; set; }
    }
}
