using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter
{
    internal static class ConsoleHelper
    {
        private static HashSet<string> affirmativeAnswers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "y",
            "yes",
            "affirmative",
            "yep",
            "uhuh",
            "true",
            "1"
        };


        public static bool PromptForConfirmation(string prompt)
        {
            Console.Write($"{prompt} (y/n):");
            string? answer = Console.ReadLine();

            return !string.IsNullOrWhiteSpace(answer) && affirmativeAnswers.Contains(answer);
        }
    }
}
