using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GettingStarted
{
    public class Message
    {
        public string Text { get; set; }
    }

    public class MessageConsumer :
        IConsumer<Message>
    {
        readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            await context.Publish(new Event(context.Message.Text), context.CancellationToken);
            _logger.LogInformation("Received Text: {Text}", context.Message.Text);

            throw new Exception("Something went wrong");
        }
    }
}