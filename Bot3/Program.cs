using System;


namespace Bot3;

class Program
{
    static async Task Main(string[] args)
    {
        const string Token = "7557053637:AAGS9WhqbAznqEVjmngR60lMXQvK6oNp46o";
        
        BotHandler botHandler = new BotHandler(Token);

        await botHandler.BotHandle();
    }
}

