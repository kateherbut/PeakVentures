using AutoMapper;
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Extensions.Options;
using PeakVentures.StorageService.Configurations;

namespace PeakVentures.StorageService.Services
{
    public interface IServiceBusConsumer : IAsyncDisposable
    {
        Task RegisterOnMessageHandlerAsync<TMessage, TCommand>();
        Task CloseQueueAsync();
    }

    public class ServiceBusConsumer : IServiceBusConsumer
    {
        private readonly ServiceBusConfiguration config;
        private readonly ServiceBusClient client;
        private readonly IMapper mapper;
        private readonly ILogger<ServiceBusConsumer> logger;
        private readonly IServiceScopeFactory scopeFactory;
        private List<ServiceBusProcessor> processors = new();

        public ServiceBusConsumer(IOptions<ServiceBusConfiguration> config, ILogger<ServiceBusConsumer> logger, IMapper mapper, IServiceScopeFactory scopeFactory)
        {
            this.config = config.Value;

            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            this.client = new ServiceBusClient(this.config.ConnectionString, clientOptions);

            this.logger = logger;
            this.mapper = mapper;
            this.scopeFactory = scopeFactory;
        }

        public async Task RegisterOnMessageHandlerAsync<TMessage, TCommand>()
        {
            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 10,
                PrefetchCount = 20,
            };
            var processor = this.client.CreateProcessor(this.config.QueueName, options);
            processor.ProcessMessageAsync += ProcessMessagesAsync<TMessage, TCommand>;
            processor.ProcessErrorAsync += ProcessErrorAsync;
            await processor.StartProcessingAsync();

            this.processors.Add(processor);
        }

        public async Task CloseQueueAsync()
        {
            foreach (var processor in this.processors)
            {
                await processor.CloseAsync().ConfigureAwait(false);
            }
        }

        private async Task ProcessMessagesAsync<TMessage, TCommand>(ProcessMessageEventArgs args)
        {
            try
            {
                var myPayload = args.Message.Body.ToObjectFromJson<TMessage>();
                var command = this.mapper.Map<TCommand>(myPayload);
                using (var scope = this.scopeFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Publish(command);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error trying to process message from Azure Service Bus");
            }
            
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            this.logger.LogError(arg.Exception, "Message handler encountered an exception");
            this.logger.LogDebug("- ErrorSource: {errorSource}", arg.ErrorSource);
            this.logger.LogDebug("- Entity Path: {entityPath}", arg.EntityPath);
            this.logger.LogDebug("- FullyQualifiedNamespace: {fullyQualifiedNamespace}", arg.FullyQualifiedNamespace);

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var processor in this.processors)
            {
                if (processor != null)
                {
                    await processor.DisposeAsync().ConfigureAwait(false);
                }
            }

            if (this.client != null)
            {
                await this.client.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
