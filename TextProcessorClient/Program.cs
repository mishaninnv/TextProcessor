using TextProcessorClient;

var test = new string[] { "127.0.0.1", "8888" }; // тестовые данные

if (test.Length == 2
    && int.TryParse(test[1], out var port))
{
    var client = new Client(test[0], port);
    client.Start();
}
else 
{
    Console.WriteLine("Передайте значение пути и порта.");
}