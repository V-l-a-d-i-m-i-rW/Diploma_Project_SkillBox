using SkillProfi_TelegramBot.Classes;
using System;

namespace SkillProfi_TelegramBot
{
    class Program
    {
        static void Main()
        {
            try
            {
                TelegramBot tb = new();
                tb.Start();
                Console.ReadKey();
            }
            catch (ArgumentNullException ex)
            {
                TelegramBot.AddError(ex.InnerException?.Message ?? ex.Message);
                Console.ReadLine();
            }
        }
    }
}
