namespace Chat.Installers;

public static class ServiceInstaller
{
    public static void AddServiceConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwagger();
        services.AddAuthenticationServices();
        services.AddDependencies();
        services.AddDatabase(configuration);
        services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.WithOrigins("http://localhost:8080", "https://localhost:8080", "http://localhost:8081", "https://localhost:8081")
                                  .AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .AllowCredentials());
        });
        services.AddSignalR();
        services.AddAuthorization();
    }
}
