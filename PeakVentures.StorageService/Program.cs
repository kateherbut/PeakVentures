using MediatR;
using Microsoft.Extensions.Options;
using PeakVentures.StorageService.Configurations;
using PeakVentures.StorageService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ServiceBusConfiguration>(builder.Configuration.GetSection("ServiceBus"));
builder.Services.Configure<StorageConfiguration>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<IServiceBusConsumer, ServiceBusConsumer>();
builder.Services.AddSingleton<ILogBuilder, LogBuilder>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

await app.Services.RegisterQueueProcessors();

var storageConfig = app.Services.GetRequiredService<IOptions<StorageConfiguration>>().Value;
var directory = Path.GetDirectoryName(storageConfig.FilePath);
if (directory != null)
    Directory.CreateDirectory(directory);

app.Run();