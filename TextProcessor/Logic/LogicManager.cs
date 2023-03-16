using TextProcessor.DataBase;
using TextProcessor.Models;

namespace TextProcessor.Logic;

/// <summary>
/// Класс содержащий основную логику.
/// </summary>
internal class LogicManager
{
    private readonly Dictionary<string, Action> _methods;
    private readonly ManagerDb _managerDb;
    private List<WordModel> _wordList;

    public LogicManager(ManagerDb managerDb)
    {
        _methods = new Dictionary<string, Action>
        {
            { nameof(Create), Create },
            { nameof(Update), Update },
            { nameof(Delete), Delete }
        };
        _managerDb = managerDb;
        _wordList = new List<WordModel>();
    }

    /// <summary>
    /// Запуск задачи ожидающей ввода пользователя в консоль
    /// </summary>
    internal Task StartUserInput() =>
        Task.Run(() =>
        {
            while (true)
            {
                var userInput = Console.ReadLine();
                var values = userInput?.Split(" ");
                if (values?.Length > 0 && values?.Length <= 2)
                {
                    if (values.Length == 2)
                    {
                        var listManager = new ListManager(values[1]);
                        _wordList = listManager.WordList;
                    }
                    var callMethod = _methods[values[0]];
                    callMethod?.Invoke();
                }
                else
                {
                    Console.WriteLine("Введите корректные данные.");
                }
            }
        });

    /// <summary>
    /// Запуск сервера по заданному порту.
    /// </summary>
    /// <param name="port"> Значение порта по которому будет слушать сервер. </param>
    internal static Task StartServer(int port) =>
        Task.Run(() =>
        {
            var server = new Server(port);
            server.Start();
        });

    private void Create() => _managerDb.CreateList(_wordList);
    private void Update() => _managerDb.UpdateList(_wordList);
    private void Delete() => _managerDb.DeleteList();
}