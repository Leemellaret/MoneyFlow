using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoneyFlow.TelegramBot.TelegramInfrastructure;
using System.Net.NetworkInformation;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((HostBuilderContext context, IServiceCollection services) =>
    {
        services.AddHttpClient("TelegramBotClient")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                const string botTokenValueKey = "BotToken";

                var token = context.Configuration.GetValue<string>(botTokenValueKey);
                if (token == null)
                    throw new Exception($"'{botTokenValueKey}' undefined or contains null or empty string");

                var options = new TelegramBotClientOptions(token);

                return new TelegramBotClient(options, httpClient);
            });

        services.AddOptions<ReceiverOptions>()
            .Configure(opts =>
            {
                opts.ThrowPendingUpdates = false;
                opts.AllowedUpdates = new UpdateType[] { UpdateType.Message };
            });

        services.AddSingleton<IUpdateHandler, TelegramUpdatesHandler>();


        services.AddHostedService<PollingService>();
    })
    .ConfigureHostConfiguration(builder =>
    {
        builder.AddUserSecrets<Program>();
    })
    .ConfigureLogging((ILoggingBuilder lb) =>
    {
        lb.ClearProviders();
        lb.AddSimpleConsole(opts =>
        {
            opts.SingleLine = true;
            opts.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
        });
    })
    .Build();

await host.RunAsync();
