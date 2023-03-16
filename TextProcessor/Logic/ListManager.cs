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
    internal List<WordModel> WordList { get; private set; }

    /// <summary>
    /// Класс отвечающий за работу со списком слов
    /// </summary>
    /// <param name="pathToFile"> Полный путь к файлу со списком слов. </param>
    internal ListManager(string pathToFile)
    {
        WordList = new List<WordModel>();
        SetWordsFromFile(pathToFile);
    }

    /// <summary>
    /// Запись в лист слов, содержащихся в документе (не менее 3х и не более 
    /// 15ти символов, встречающиеся не менее 3х раз).
    /// </summary>
    /// <param name="pathToFile"> Полный путь к документу. </param>
    /// <exception cref="FileNotFoundException"> 
    /// Выбрасывается при отсутствии файла по заданному пути 
    /// </exception>
    private void SetWordsFromFile(string pathToFile)
    {
        if (!File.Exists(pathToFile))
        {
            throw new FileNotFoundException();
        }

        var textFromFile = File.ReadAllText(pathToFile, System.Text.Encoding.UTF8);

        WordList = Regex.Matches(textFromFile, "[a-zA-Zа-яёА-ЯЁ]{3,}")
            .Where(x => x.Length < 15)
            .Select(x => x.ToString().ToLower())
            .GroupBy(x => x)
            .Select(x => new WordModel { Word = x.Key, Count = x.Count() })
            .ToList();
    }
}
