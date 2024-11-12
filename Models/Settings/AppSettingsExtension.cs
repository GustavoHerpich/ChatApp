namespace Chat.Models.Settings;

public static class AppSettingsExtension
{
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AppSettings>()
            .Bind(configuration.GetSection(AppSettings.SectionName))
            .ValidateOnStart()
            .ValidateDataAnnotations();

        return services;
    }
}
