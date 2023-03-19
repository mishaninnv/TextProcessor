using System.Text;
using System.Text.RegularExpressions;
using TextProcessor.Models;

namespace TextProcessor.Logic;

/// <summary>
/// Класс отвечающий за работу со списком слов
/// </summary>
internal class ListManager
{
    /// <summary>
    /// Список слов
    /// </summary>
    internal static List<WordModel> WordList { get; private set; } = new List<WordModel>();

    /// <summary>
    /// Запись в список слов, содержащихся в документе (не менее 3х и не более 
    /// 15ти символов, встречающиеся не менее 3х раз).
    /// </summary>
    /// <param name="pathToFile"> Полный путь к документу. </param>
    /// <exception cref="FileNotFoundException"> 
    /// Выбрасывается при отсутствии файла по заданному пути 
    /// </exception>
    internal static void SetWordsFromFile(string pathToFile)
    {
        if (!File.Exists(pathToFile))
        {
            throw new FileNotFoundException();
        }

        if (GetFileEncoding(pathToFile) == Encoding.UTF8)
        {
            var textFromFile = File.ReadAllText(pathToFile, Encoding.UTF8);

            WordList = Regex.Matches(textFromFile, "[a-zA-Zа-яёА-ЯЁ]{3,}")
                .Where(x => x.Length <= 15)
                .Select(x => x.ToString().ToLower())
                .GroupBy(x => x)
                .Select(x => new WordModel { Word = x.Key, Count = x.Count() })
                .ToList();
        }
        else
        {
            throw new FormatException("Неверная кодировка файла.");
        }

    }


    /// <summary>
    /// Получить кодировку файла.
    /// </summary>
    /// <param name="pathToFile"></param>
    /// <returns></returns>
    private static Encoding GetFileEncoding(string pathToFile)
    {

        using (var fs = File.OpenRead(pathToFile))
        {
            Ude.CharsetDetector cdet = new Ude.CharsetDetector();
            cdet.Feed(fs);
            cdet.DataEnd();
            var fileEncoding = cdet.Charset;
            if (fileEncoding.Equals("UTF-8"))
            {
                return Encoding.UTF8;
            }
            return Encoding.Default;
        }
    }
}
