namespace UserService.Api.Init;

public static class ApiInitExtensions
{
    public static IServiceCollection AddServiceApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
