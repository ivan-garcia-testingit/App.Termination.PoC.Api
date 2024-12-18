using App.Termination.PoC.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddScoped<OperationService>()
    .AddScoped<EnvironmentService>()
    .AddSingleton<GuidService>();

var app = builder.Build();

app.Lifetime.ApplicationStopping.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("[App.Termination.PoC.Api_ApplicationStopping] Waiting for 15 seconds to stop traffic");
    Thread.Sleep(15000);
    var guids = scope.ServiceProvider.GetRequiredService<GuidService>();
    logger.LogInformation("[App.Termination.PoC.Api_ApplicationStopping] Waiting for all tasks to finish");
    guids.WaitForAllGuids().Wait();
    logger.LogInformation("[App.Termination.PoC.Api_ApplicationStopping] Finished waiting for all tasks");
});

app.Lifetime.ApplicationStopped.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("[App.Termination.PoC.Api_ApplicationStopped] Now stopping the application");
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
