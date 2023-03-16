using TextProcessor.DataBase;
using TextProcessor.Logic;

var test = new string[] { "localhost", "1433" }; // тестовые данные

if (test.Length == 2 && int.TryParse(test[1], out var port))
{
    var managerDb = new ManagerDb(test[0]);
    managerDb.InitDb();
    var manager = new LogicManager(managerDb);
    // передать параметры которые получили от пользователя
    // возможно надо как то записать в апп конфиг строку подключения
    manager.StartUserInput();
    LogicManager.StartServer(port);
}