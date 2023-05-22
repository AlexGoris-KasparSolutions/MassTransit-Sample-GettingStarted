using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GettingStarted
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<MessageConsumer>(cfg => //This one is throwing the exception
                        {
                            cfg.UseDelayedRedelivery(r => r.Interval(2, TimeSpan.FromSeconds(5)));
                        });

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseConsumeFilter(typeof(LogFilter<>), context);
                            cfg.ConfigureEndpoints(context);
                        });
                    });

                    services.AddHostedService<Worker>();
                });
    }
}
