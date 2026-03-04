namespace FinanceService.Api.Init;

public static class PipelineInitExtensions
{
    public static IApplicationBuilder UsePipeline(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
