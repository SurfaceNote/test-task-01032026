namespace UserService.Api.Init;

public static class PipelineInitExtensions
{
    public static IApplicationBuilder UsePipeline(this WebApplication app)
    {
        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
