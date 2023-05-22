using System;
using System.Threading.Tasks;
using MassTransit;

using Microsoft.Extensions.Logging;

namespace GettingStarted
{
    /// <summary>
    ///     A filter which logs pre-consumes, post-consumes and consumer exceptions.
    /// </summary>
    /// <typeparam name="T">The type of message which the <see cref="IConsumer{TMessage}" /> consumes.</typeparam>
    public class LogFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly ILogger<LogFilter<T>> _logger;

        public LogFilter(ILogger<LogFilter<T>> logger)
        {
            _logger = logger;
        }

        public void Probe(ProbeContext context) => context.CreateFilterScope("log-filter");

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            LogPreConsume(context);
            try
            {
                await next.Send(context);
            }
            catch (Exception exception)
            {
                LogConsumeException(context, exception);
                throw;
            }

            LogPostConsume(context);
        }

        private void LogPreConsume(ConsumeContext context)
        {
            _logger.LogInformation(
                "{MessageType}:{EventType} correlated by {CorrelationId} on {Address} with send time {SentTime:dd/MM/yyyy HH:mm:ss:ffff}",
                typeof(T).Name,
                "PreConsume",
                context.CorrelationId,
                context.ReceiveContext.InputAddress,
                context.SentTime?.ToUniversalTime());
        }

        private void LogPostConsume(ConsumeContext context)
        {
            _logger.LogInformation(
                "{MessageType}:{EventType} correlated by {CorrelationId} on {Address}"
                + " with send time {SentTime:dd/MM/yyyy HH:mm:ss:ffff}"
                + " and elapsed time {ElapsedTime}",
                typeof(T).Name,
                "PostConsume",
                context.CorrelationId,
                context.ReceiveContext.InputAddress,
                context.SentTime?.ToUniversalTime(),
                context.ReceiveContext.ElapsedTime);
        }

        private void LogConsumeException(ConsumeContext<T> context, Exception exception)
            => LogConsumeProblem(context, exception, LogLevel.Error);

        private void LogConsumeProblem(ConsumeContext<T> context, Exception exception, LogLevel level)
        {
            _logger.Log(level, exception,
                "{MessageType}:{EventType} correlated by {CorrelationId} on {Address}"
                + " with sent time {SentTime:dd/MM/yyyy HH:mm:ss:ffff}"
                + " and elapsed time {ElapsedTime}"
                + " and message {@message}",
                typeof(T).Name,
                "ConsumeFailure",
                context.CorrelationId,
                context.ReceiveContext.InputAddress,
                context.SentTime?.ToUniversalTime(),
                context.ReceiveContext.ElapsedTime,
                context.Message);
        }
    }
}