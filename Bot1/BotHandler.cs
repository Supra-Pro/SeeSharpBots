using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot1;

public class BotHandler
{
    public string Token { get; set; }

    public BotHandler(string token)
    {
        Token = token;
    }
    
    
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

            // DC
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



    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update?.Message?.From == null)
            return;
        
        User user = new User
        {
            id = update.Message.From.Id,
            Name = update.Message.From.FirstName,
            Username = $"@{update.Message.From.Username}",
            PremiumBomi = update.Message.From.IsPremium ?? false
        };
            
        if (update.Message?.Type == MessageType.Text)
        {
            Message message = new Message
            {
                Text = update.Message.Text,
                Date = update.Message.Date
            };

            var premiummi = "yo'q";

            if (user.PremiumBomi == true)
            {
                premiummi = "bor";
            }

            await botClient.SendTextMessageAsync(
                chatId: "@alo_xashar",
                text: $"\ud83d\udcac Yangi xabar:\n" +
                      $"Name: {user.Name}\n" +
                      $"Username: {user.Username}\n" +
                      $"ID: {user.id}\n" +
                      $"Xabar: {message.Text}\n" +
                      $"Vaqt: {message.Date}\n" +
                      $"Premium: {premiummi}",
                cancellationToken: cancellationToken
            );

            if (message.Text == "/start" || message.Text == "start")
            {
                if (user.id == 8143337642)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: user.id,
                        text: "Bo'lar endi!",
                        cancellationToken: cancellationToken
                    );
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: user.id,
                        text: "\ud83d\udd3c Xush kepsiz! \ud83d\udd3d",
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: user.id,
                text: "Iltimos, faqat text yuboring!!!",
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