using TextProcessor.DataBase;
using TextProcessor.Models;

namespace TextProcessor.Logic;

internal class LogicManager
{
    private readonly Dictionary<string, Action> _methods;
    private readonly ManagerDb _managerDb;
    private List<WordModel> _wordList;

    public LogicManager(ManagerDb managerDb)
    {
        _methods = new Dictionary<string, Action>
        {
            { "Create", Create },
            { "Update", Update },
            { "Delete", Delete }
        };
        _managerDb = managerDb;
        _wordList = new List<WordModel>();
    }

    internal Task StartUserInput()
    {
        return Task.Run(() =>
        {
            while (true)
            {
                var userInput = Console.ReadLine(); // проверить пустую строку
                var values = userInput?.Split(" ");
                if (values?.Length > 0 && values?.Length <= 2)
                {
                    if (values.Length == 2)
                    {
                        var listManager = new ListManager(values[1]);
                        _wordList = listManager.WordList;
                    }
                    var callMethod = _methods[values[0]];
                    callMethod?.Invoke(); // проверить отствие пераданного пути листа
                }
                else
                {
                    Console.WriteLine("Введите корректные данные.");
                }
            }
        });
    }
    internal static Task StartServer(int port)
    {
        return Task.Run(() =>
        {
            var server = new Server(port);
            server.Start();
        });
    }

    private void Create() => _managerDb.CreateList(_wordList);
    private void Update() => _managerDb.UpdateList(_wordList);
    private void Delete() => _managerDb.DeleteList();
}