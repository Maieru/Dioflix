using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.Configure<KestrelServerOptions>(opt =>
{
    opt.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});

builder.Services.AddSingleton<CosmosClient>(s =>
{
    var connectionString = Environment.GetEnvironmentVariable("CosmosDbConnection");
    return new CosmosClient(connectionString);
});

builder.Build().Run();
