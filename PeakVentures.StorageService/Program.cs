using MediatR;
using Microsoft.Extensions.Options;
using PeakVentures.StorageService.Configurations;
using PeakVentures.StorageService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ServiceBusConfiguration>(builder.Configuration.GetSection("ServiceBus"));
builder.Services.Configure<StorageConfiguration>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<IServiceBusConsumer, ServiceBusConsumer>();
builder.Services.AddSingleton<ILogBuilder, LogBuilder>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.Services.RegisterQueueProcessors();

var storageConfig = app.Services.GetRequiredService<IOptions<StorageConfiguration>>().Value;
var directory = Path.GetDirectoryName(storageConfig.FilePath);
if (directory != null)
    Directory.CreateDirectory(directory);

app.UseHttpsRedirection();

app.Run();