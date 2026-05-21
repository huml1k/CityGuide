using ContentService.Domain.Entities;

namespace ContentService.Application.Features.Routes;

internal static class RouteCoverImageMapper
{
    public static (Guid? Id, string? Extension) GetCover(IEnumerable<RouteImage>? images)
    {
        if (images is null)
        {
            return (null, null);
        }

        var cover = images.FirstOrDefault(x => x.IsCover)
            ?? images.OrderBy(x => x.OrderIndex).FirstOrDefault();

        return cover is null ? (null, null) : (cover.Id, cover.FileExtension);
    }
}
