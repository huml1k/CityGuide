namespace AdminService.Application.Options;

public class CityGuideServicesOptions
{
    public const string SectionName = "CityGuideServices";

    public string AuthService { get; set; } = string.Empty;

    public string UserService { get; set; } = string.Empty;

    public string ContentService { get; set; } = string.Empty;

    public string NotificationService { get; set; } = string.Empty;

    public string ApiGateway { get; set; } = string.Empty;
}
