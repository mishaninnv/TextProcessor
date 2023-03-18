using TextProcessor.DataBase;
using TextProcessor.Logic;
using TextProcessor.Models;
using TextProcessor.Servers;

if (args.Length == 2 && int.TryParse(args[1], out var port))
{
    var managerDb = new SqlServerDbManager(args[0]);

    var server = new TcpIpServer(managerDb);
    var serverSettings = new ServerSettingsModel() { Port = port };
    LogicManager.StartServer(server, serverSettings);

    var manager = new LogicManager(managerDb);
    await manager.StartUserInput();
}