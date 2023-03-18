using TextProcessorClient.Clients;
using TextProcessorClient.Logic;
using TextProcessorClient.Models;

if (args.Length == 2 && int.TryParse(args[1], out var port))
{
    var settings = new ClientSettingsModel() { Host = args[1], Port = port };
    var clientManager = new ClientsManager(new TcpIpClient(), settings);
    clientManager.StartClient();
}
else 
{
    Console.WriteLine("Передайте значение пути и порта.");
}