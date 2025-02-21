using System;

namespace SeeSharpBotlessons
{
    class Program
    {
        static void Main(string[] args)
        {
            const string Token = "8005023235:AAEea2xs-Zr6ISicSDjZcDFKSrg9z-UiFB0";

            botHanler bot = new botHanler(Token);

            bot.BotHandle();
        }
    }
}

