using System.Text.RegularExpressions;
using TextProcessor.Models;

namespace TextProcessor.Logic
{
    internal class FileManager
    {
        internal List<WordModel> GetWordsFromFile(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException();
            }

            var textFromFile = File.ReadAllText(pathToFile);

            var wordList = Regex.Matches(textFromFile, "[a-zA-Zа-яёА-ЯЁ]{3,}")
                .Where(x => x.Length < 15)
                .Select(x => x.ToString())
                .GroupBy(x => x)
                .Where(x => x.Count() > 3)
                .Select(x => new WordModel { Word = x.Key, Count = x.Count() })
                .ToList();

            return wordList;
        }
    }
}
