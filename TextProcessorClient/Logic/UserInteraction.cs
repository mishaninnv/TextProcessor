using System.Text;

namespace TextProcessorClient.Logic;

internal class UserInteraction
{
    /// <summary>
    /// Ожидание пользовательского ввода.
    /// </summary>
    /// <returns> Строка введенная пользователем. </returns>
    internal static string GetUserInput()
    {
        var retString = new StringBuilder();
        while (true)
        {
            var readKeyResult = Console.ReadKey(true);

            if (readKeyResult.Key == ConsoleKey.Escape)
            {
                return string.Empty;
            }
            else if (readKeyResult.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (readKeyResult.Key == ConsoleKey.Backspace && retString.Length > 0)
            {
                retString.Remove(retString.Length - 1, 1);
                Console.Write("\b \b");
            }
            else
            {
                retString.Append(readKeyResult.KeyChar);
                Console.Write(readKeyResult.KeyChar);
            }
        }
        return retString.ToString();
    }
}
