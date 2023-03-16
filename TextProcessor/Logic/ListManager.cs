using System.Text.RegularExpressions;
using TextProcessor.Models;

namespace TextProcessor.Logic;

internal class ListManager
{
    private List<WordModel> _wordList;
    internal List<WordModel> WordList 
        { get => _wordList ?? throw new ArgumentNullException("Отсутствует список."); }

    internal ListManager(string pathToFile)
    {
        _wordList = new List<WordModel>();
        SetWordsFromFile(pathToFile);
    }

    private void SetWordsFromFile(string pathToFile)
    {
        if (!File.Exists(pathToFile))
        {
            throw new FileNotFoundException();
        }

        var textFromFile = File.ReadAllText(pathToFile, System.Text.Encoding.UTF8);

        _wordList = Regex.Matches(textFromFile, "[a-zA-Zа-яёА-ЯЁ]{3,}")
            .Where(x => x.Length < 15)
            .Select(x => x.ToString().ToLower())
            .GroupBy(x => x)
            .Select(x => new WordModel { Word = x.Key, Count = x.Count() })
            .ToList();
    }
}
