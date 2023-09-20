using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoneyFlow.TelegramBot.TelegramInfrastructure
{
    internal class PollingService : IHostedService
    {
        private readonly ILogger<PollingService> _logger;
        private readonly ReceiverOptions _options;
        private readonly ITelegramBotClient _botClient;
        private readonly IUpdateHandler _updateHandler;

        private readonly CancellationTokenSource _cts;
        private Task? _pollingProcessTask;

        public PollingService(
            ILogger<PollingService> logger,
            IOptions<ReceiverOptions> options,
            ITelegramBotClient botClient,
            IUpdateHandler updateHandler
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
            _updateHandler = updateHandler ?? throw new ArgumentNullException(nameof(updateHandler));
            _cts = new();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _pollingProcessTask = Task.Run(() => StartReceiving(_cts.Token), cancellationToken);

            return Task.CompletedTask;
        }

        private async Task StartReceiving(CancellationToken token)
        {
            _logger.LogInformation("About to start receiving updates");
            await _botClient.ReceiveAsync(_updateHandler, _options, token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            _logger.LogInformation("Request to cancel receiving updates");

            if (_pollingProcessTask != null)
            {
                await _pollingProcessTask;
            }
            _logger.LogInformation("Receiving updates stopped");

            _cts.Dispose();
        }
    }
}
