using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace MoneyFlow.TelegramBot.TelegramInfrastructure
{
    internal class TelegramUpdatesHandler : IUpdateHandler
    {
        private readonly ILogger<TelegramUpdatesHandler> _logger;

        public TelegramUpdatesHandler(ILogger<TelegramUpdatesHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received update: type={Type}", update.Type);

            if (update.Message == null)
                return;

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Echo", replyToMessageId: update.Message.MessageId);
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Polling failed");
            return Task.CompletedTask;
        }
    }
}
