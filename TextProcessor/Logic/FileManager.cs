using System.Text.RegularExpressions;
using TextProcessor.Models;

namespace TextProcessor.Logic;

/// <summary>
/// Класс содержащий методы для работы с файлами.
/// </summary>
internal class FileManager
{
    /// <summary>
    /// Получение из файла данных, с приведением их к List<WordModel>.
    /// </summary>
    /// <param name="pathToFile"> Путь к файлу с данными </param>
    /// <returns> Сгруппированные данные полученные из файла. </returns>
    /// <exception cref="FileNotFoundException"> При отсутсвии файла по заданному пути. </exception>
    internal List<WordModel> GetWordsFromFile(string pathToFile)
    {
        if (!File.Exists(pathToFile))
        {
            throw new FileNotFoundException();
        }

        var textFromFile = File.ReadAllText(pathToFile);

        var wordList = Regex.Matches(textFromFile, "[a-zA-Zа-яёА-ЯЁ]{3,}")
            .Where(x => x.Length < 15)
            .Select(x => x.ToString().ToLower())
            .GroupBy(x => x)
            .Select(x => new WordModel { Word = x.Key, Count = x.Count() })
            .ToList();

        return wordList;
    }
}
