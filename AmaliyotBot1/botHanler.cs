using System.Net.Mime;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SeeSharpBotlessons;

public class botHanler
{
    public botHanler(string token)
    {
        Token = token;
    }

    public string Token { get; set; }

    public async Task BotHandle()
    {
        try
        {
            var botClient = new TelegramBotClient(Token);

            using var cts = new CancellationTokenSource();

            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }


    private async Task HandleUpdateAsync(ITelegramBotClient botClient,Update update, CancellationToken cancellationToken)
    {
        User user = new User()
        {
            id = update.Message.From.Id,
            name = update.Message.From.FirstName,
            Username = $"@{update.Message.From.Username}"
        };
        if (update.Message?.Type == MessageType.Text)
        {
            Message message = new Message
            {
                Text = update.Message.Text,
                Deta = update.Message.Date
            };
                await botClient.SendTextMessageAsync(
                    chatId: "@salomNotnot"
                    text: $"\ud83\udcac Yangi xabar:" +
                          $"Name:"
                    )
            
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: user.id,
                text: "Text jo'nat bot Text deyapman bot bot bot!!!!!!!!!!!",
                cancellationToken: cancellationToken
                
            );
        }
    }
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        try
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine($"Error in polling: {errorMessage}");

            if (exception is ApiRequestException apiEx && apiEx.ErrorCode == 403)
            {
                Console.WriteLine("User has blocked the bot.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error handling polling error: {e}");
        }

        return Task.CompletedTask;
    }
}