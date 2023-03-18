using TextProcessor.DataBase;
using TextProcessor.Models;
using TextProcessor.Servers;

namespace TextProcessor.Logic;

/// <summary>
/// Класс содержащий основную логику.
/// </summary>
internal class LogicManager
{
    private readonly Dictionary<string, Action> _methods;
    private readonly IDbManager _managerDb;

    public LogicManager(IDbManager managerDb)
    {
        _methods = new Dictionary<string, Action>
        {
            { nameof(Create), Create },
            { nameof(Update), Update },
            { nameof(Delete), Delete }
        };
        _managerDb = managerDb;
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
                var values = userInput?.Split(" ")
                                       .Where(x => x.Length > 0)
                                       .ToArray();
                if (values?.Length > 0 && values?.Length <= 2)
                {
                    if (values.Length == 2)
                    {
                        try 
                        {
                            ListManager.SetWordsFromFile(values[1]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            continue;
                        }
                    }
                    if (_methods.Keys.Contains(values[0]))
                    {
                        var callMethod = _methods[values[0]];
                        callMethod?.Invoke();
                    }
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
    internal static Task StartServer(IServer server, ServerSettingsModel settings) =>
        Task.Run(() =>
        {
            server.Start(settings);
        });

    private void Create() => _managerDb.CreateList(ListManager.WordList);
    private void Update() => _managerDb.UpdateList(ListManager.WordList);
    private void Delete() => _managerDb.DeleteList();
}