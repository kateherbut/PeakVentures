using MediatR;
using Microsoft.AspNetCore.Mvc;
using PeakVentures.PixelService.Configurations;
using PeakVentures.PixelService.Constants;
using PeakVentures.PixelService.Core.UserResponse;
using PeakVentures.PixelService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddSingleton<IServiceBusClientFactory, ServiceBusClientFactory>();
builder.Services.Configure<ServiceBusConfiguration>(builder.Configuration.GetSection("ServiceBus"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/pixel-service/track", async ([FromHeader(Name = "referer")] string referer, [FromHeader(Name = "user-agent")] string userAgent, HttpContext context, IMediator mediator) =>
{
    await mediator.Publish(new PublishUserResponseCommand {
        IpAddress = context.Connection.RemoteIpAddress.ToString(),
        Referer = referer,
        UserAgent = userAgent
    });

    return Results.File(Image.TransparentPixel, contentType: Image.MimeType);
})
.WithName("TrackUserResponse");

app.Run();
