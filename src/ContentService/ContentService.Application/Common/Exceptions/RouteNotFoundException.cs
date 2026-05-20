namespace ContentService.Application.Common.Exceptions
{
    public class RouteNotFoundException : NotFoundException
    {
        public RouteNotFoundException(Guid routeId)
            : base($"Route with id '{routeId}' was not found.")
        {
        }
    }
}
