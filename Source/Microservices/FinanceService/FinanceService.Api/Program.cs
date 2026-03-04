using FinanceService.Api.Init;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddServiceApi()
    .AddAuthentication(builder.Configuration)
    .AddApplication();

var app = builder.Build();

app.UseExceptionHandling();
app.UsePipeline();

app.Run();
