using TextProcessor.DataBase;
using TextProcessor.Logic;

if (args.Length == 2 && int.TryParse(args[1], out var port))
{
    var managerDb = new ManagerDb(args[0]);
    var manager = new LogicManager(managerDb);
    manager.StartUserInput();
    LogicManager.StartServer(port);
}