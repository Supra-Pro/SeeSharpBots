using System;

namespace Bot2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string Token = "8186876636:AAGFhMDa9L6GWP0y4qp-ehEkGVSeuaD390Q";

            BotHandler botHandler = new BotHandler(Token);

            await botHandler.BotHandle();

        }
    }
}

