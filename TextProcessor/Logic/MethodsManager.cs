using System.Text;
using TextProcessor.DataBase;
using TextProcessor.Infrastructure.Enums;
using TextProcessor.Models;

namespace TextProcessor.Logic;

/// <summary>
/// Класс содержащий методы общей логики приложения.
/// </summary>
internal class MethodsManager
{
    private readonly Dictionary<string, Action> _methods;
    private List<WordModel> _wordList;
    private readonly ManagerDb _managerDb;
    private readonly FileManager _fileManager;

    public MethodsManager()
    {
        _methods = new Dictionary<string, Action>
        {
            { nameof(MethodEnumerator.Create), Create },
            { nameof(MethodEnumerator.Update), Update },
            { nameof(MethodEnumerator.Delete), Delete }
        };
        _wordList = new List<WordModel>();
        _managerDb = new ManagerDb();
        _fileManager = new FileManager();
    }

    /// <summary>
    /// Получение данных автодополнения из словаря.
    /// </summary>
    internal void GetData()
    {
        var retString = new StringBuilder();
        do
        {
            var readKeyResult = Console.ReadKey(true);

            if (readKeyResult.Key == ConsoleKey.Escape)
            {
                break;
            }
            else if (readKeyResult.Key == ConsoleKey.Enter)
            {
                var userInput = retString.ToString();
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    break;
                }
                var result = _managerDb.GetWords(userInput);
                foreach (var word in result)
                {
                    Console.WriteLine(word.Word);
                }
                Console.WriteLine();
                retString.Clear();
            }
            else
            {
                retString.Append(readKeyResult.KeyChar);
                Console.Write(readKeyResult.KeyChar);
            }
        }
        while (true);
    }

    /// <summary>
    /// Вызов метода из словаря по имени.
    /// </summary>
    /// <param name="methodName"> Имя метода. </param>
    internal void CallMethod(string methodName)
    {
        var callMethod = _methods[methodName];
        callMethod?.Invoke();
    }

    /// <summary>
    /// Вызов метода из словаря по имени. Получение данных из файла.
    /// </summary>
    /// <param name="methodName"> Имя метода. </param>
    /// <param name="path"> Путь к файлу с данными. </param>
    internal void CallMethod(string methodName, string path)
    {
        _wordList = _fileManager.GetWordsFromFile(path);
        if (_wordList.Any())
        {
            CallMethod(methodName);
        }
    }

    private void Create() => _managerDb.CreateList(_wordList);
    private void Update() => _managerDb.UpdateList(_wordList);
    private void Delete() => _managerDb.DeleteList();
}
