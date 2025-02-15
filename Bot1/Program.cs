using System;

namespace Bot1
{
    class Program
    {
        static async Task Main(string[] args)
        {

            const string Token = "8186876636:AAHEUQIAyInVK7cl3_5N2H53ypJPlQRxPqQ";

            BotHandler handler = new BotHandler(Token);

            try
            {
                await handler.BotHandle();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

        }
    }
}