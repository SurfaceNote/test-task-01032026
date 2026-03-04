using UserService.Api.Init;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddServiceApi()
    .AddApplication(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandling();
app.UsePipeline();

app.Run();
