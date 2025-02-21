using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot2;

public class BotHandler
{
    public string? Token { get; set; }
    // O'zgaruvchilar

    public long[] Admins = { 5091219046 };
    public long[] Spams = { };


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
                AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
            };

            // miyasi
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

    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        
        if (update.CallbackQuery != null)
        {
            await HandleCallbackQuery(botClient, update.CallbackQuery, cancellationToken);
            return;
        }
        
        
        // bot foydalanuvchisi hali ham aktivligiga tekshiradi
        if (update?.Message?.From == null)
            return;

        User user = new User
        {
            Id = update.Message.From.Id,
            Username = update.Message.From.Username,
            Name = update.Message.From.FirstName
        };

        // Alisher bilan Afruzbekni bloklab qoydik
        if (Spams.Contains(user.Id))
            return;

        
        // tekstni filterdan o'tkazyapmiz
        if (update?.Message.Type == MessageType.Text)
        {

            Message message = new Message
            {
                Date = update.Message.Date,
                Text = update.Message.Text
            };

            var adminga_xabar = $"Yangi xabar:\n\n" +
                                $"Text: {message.Text}\n" +
                                $"Sana: {message.Date}\n\n" +
                                $"User: {user.Name}\n" +
                                $"ID: {user.Id}\n" +
                                $"Username: @{user.Username}";
            
            foreach (var a in Admins)
            {
                await botClient.SendTextMessageAsync(
                    chatId: a,
                    text: adminga_xabar,
                    cancellationToken: cancellationToken
                );
            }

            if (message.Text == "/start")
            {

                ReplyKeyboardMarkup btn = new ReplyKeyboardMarkup(new []
                {
                    KeyboardButton.WithRequestContact("Contact"), 
                    new KeyboardButton("About"),
                    KeyboardButton.WithRequestLocation("Location"), 
                })
                {
                    ResizeKeyboard = true
                };
                
                
                await botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: "Welcome!!!\n\nTugmacha tanlang!",
                    replyMarkup: btn,
                    cancellationToken: cancellationToken
                );
            }
            else if (message.Text.StartsWith("/post") && Admins.Contains(user.Id))
            {
                string postContent = message.Text.Replace("/post", "").Trim();
                if (string.IsNullOrWhiteSpace(postContent))
                {
                    await botClient.SendTextMessageAsync(user.Id, "❌ Postni matnini kiriting: `/post Sizning matningiz`", parseMode: ParseMode.Markdown, cancellationToken: cancellationToken);
                    return;
                }

                InlineKeyboardMarkup buttons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("Real Madrid", "like"),
                    InlineKeyboardButton.WithCallbackData("Barcelona", "dislike"),
                });

                await botClient.SendTextMessageAsync("@alo_xashar", postContent, replyMarkup: buttons, cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(user.Id, "✅ Post muvaffaqiyatli kanalga yuborildi!", cancellationToken: cancellationToken);
            }
            
            else if (message.Text == "About")
            {
                InlineKeyboardMarkup btn = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("👍", "like"),
                    InlineKeyboardButton.WithCallbackData("👎", "dislike"),
                });


                await botClient.SendPhotoAsync(
                    chatId: user.Id,
                    photo:
                    InputFile.FromUri("https://images.unsplash.com/photo-1739862836703-03eca4457f77?q=80&w=2731&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"),
                    caption: "Bu bizi maktab",
                    replyMarkup: btn,
                    cancellationToken: cancellationToken
                );

            }
            
        }
        // Agar foydalanuvchi telefon jo'natsa, kiradi...
        else if (update?.Message.Type == MessageType.Contact)
        {
            var contact = update.Message.Contact;

            string adminga_xabar = $"Yangi contact:\n\n" +
                                   $"Phone: {contact.PhoneNumber}\n" +
                                   $"Name: {contact.FirstName}\n" +
                                   $"ID: {contact.UserId}\n";

            foreach (var a in Admins)
            {
                await botClient.SendTextMessageAsync(
                    chatId: a,
                    text: adminga_xabar,
                    cancellationToken: cancellationToken
                );
            }
        }
        
        // Xabar lokatsiya bo'sa, keyin kiradi
        else if (update?.Message.Type == MessageType.Location)
        {
            var location = update.Message.Location;

            foreach (var a in Admins)
            {
                await botClient.SendLocationAsync(
                    chatId: a,
                    longitude: location.Longitude,
                    latitude: location.Latitude,
                    cancellationToken: cancellationToken
                );
                
                await botClient.SendTextMessageAsync(
                    chatId: a,
                    text: $"Yangi lokatsiya keldi\n\n" +
                          $"User: {user.Name}\n" +
                          $"Username: @{user.Username}",
                    cancellationToken: cancellationToken
                );

            }
        }
    }
    
    
    private async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callback, CancellationToken cancellationToken)
    {
        string data = callback.Data;
        User user = new User
        {
            Id = callback.From.Id,
            Username = $"@{callback.From.Username}" ?? "NoUsername",
            Name = callback.From.FirstName
        };

        string responseText = data switch
        {
            "like" => "Real",
            "dislike" => "Barca",
            _ => "❓ Noma’lum javob"
        };

        await botClient.AnswerCallbackQueryAsync(callback.Id, responseText, cancellationToken: cancellationToken);

        foreach (var admin in Admins)
        {
            await botClient.SendTextMessageAsync(
                chatId: admin,
                text: $"📢 Kimdir {responseText} bosdi!\n\n👤 {user.Name}\n🔗 Username: {user.Username}",
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