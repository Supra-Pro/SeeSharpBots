using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot3;

public class BotHandler
{
    public string Token { get; set; }
    public long[] Admins = { 5091219046 };

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
                AllowedUpdates = new UpdateType[] { UpdateType.Message, UpdateType.CallbackQuery }
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



    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update?.CallbackQuery != null)
        {
            User user = new User
            {
                Id = update.CallbackQuery.From.Id,
                Name = update.CallbackQuery.From.FirstName,
                Username = update.CallbackQuery.From.Username
            };
            await HandleCallbackQuery(user, botClient, update, cancellationToken);
            return;
        }

        if (update?.Message?.From == null)
        {
            return;
        }

        User user1 = new User
        {
            Id = update.Message.From.Id,
            Name = update.Message.From.FirstName,
            Username = update.Message.From.Username
        };

        if (update.Message.Type == MessageType.Text)
        {
            await HandleTextMessage(user1, botClient, update, cancellationToken);
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: user1.Id,
                text: "Biz faqat TEXT qa'bul qilamiz",
                cancellationToken: cancellationToken
            );
        }
    }



    public async Task HandleCallbackQuery(User user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Keldi");
        if (update.CallbackQuery == null)
        {
            return;
        }
        
        string data = update.CallbackQuery.Data;
        await botClient.AnswerCallbackQueryAsync(
            callbackQueryId: update.CallbackQuery.Id,
            cancellationToken: cancellationToken
        );
        if (string.IsNullOrEmpty(data))
        {
            return;
        }


        switch (data)
        {
            case "like":
                foreach (var admin in Admins)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: admin,
                        text: $"Kimdir 'LIKE' bosdi\n\n" +
                              $"User: {user.Name}\n" +
                              $"Username: @{user.Username}",
                        cancellationToken: cancellationToken
                    );
                }
                break;
            
            case "dislike":
                foreach (var admin in Admins)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: admin,
                        text: $"Kimdir 'DISLIKE' bosdi\n\n" +
                              $"User: {user.Name}\n" +
                              $"Username: @{user.Username}",
                        cancellationToken: cancellationToken
                    );
                }
                break;
            default:
                Console.WriteLine("Binnima boldi");
                break;
        }
    }
    
    
    public async Task HandleTextMessage(User user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update?.Message?.Text == null)
        {
            return;
        }

        Message message = new Message
        {
            Text = update.Message.Text,
            Date = update.Message.Date
        };
        


        if (message.Text.Equals("/start"))
        {
            await botClient.SendTextMessageAsync(
                chatId: user.Id,
                text: "Xush kelibsiz 🍾",
                cancellationToken: cancellationToken
            );
        }
        else if (message.Text.StartsWith("/post") && Admins.Contains(user.Id))
        {
            string postContent = message.Text.Replace("/post", "").Trim();

            if (string.IsNullOrWhiteSpace(postContent))
            {
                await botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: "❌ Xabarni bunday yozing:\n\n/post Sizning xabaringiz",
                    cancellationToken: cancellationToken
                );
                return;
            }
            else
            {
                InlineKeyboardMarkup btn = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("👍", "like"),
                    InlineKeyboardButton.WithCallbackData("👎", "dislike"),
                });

                await botClient.SendTextMessageAsync(
                    chatId: "@Otabek_Writing",
                    text: postContent,
                    replyMarkup: btn,
                    cancellationToken: cancellationToken
                );
                
                await botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: "✅ Post kanalga joylandi",
                    cancellationToken: cancellationToken
                );
            }
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