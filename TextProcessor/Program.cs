using TextProcessor.DataBase;
using TextProcessor.Logic;

var test = new string[] { "localhost", "8888" };

if (test.Length == 2 && int.TryParse(test[1], out var port))
{
    var managerDb = new ManagerDb(test[0]);
    var manager = new LogicManager(managerDb);
    LogicManager.StartServer(port);
    await manager.StartUserInput();
}