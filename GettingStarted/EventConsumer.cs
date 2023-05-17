using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GettingStarted
{
    public record Event(string Message);

    public class EventConsumer : IConsumer<Event>
    {
        private readonly ILogger<EventConsumer> _logger;

        public EventConsumer(ILogger<EventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Event> context)
        {
            _logger.LogInformation($"Received event with message: {context.Message.Message}");

            return Task.CompletedTask;
        }
    }
}