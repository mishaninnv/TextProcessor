using System.Net.Sockets;
using System.Text;

namespace TextProcessorClient;

/// <summary>
/// Класс отвечающий за работу клиента с сервером по заданному пути и порту.
/// </summary>
internal class Client
{
    private readonly string _host;
    private readonly int _port;
    private bool _isRun;

    /// <summary>
    /// Класс отвечающий за работу клиента с сервером по заданному пути и порту.
    /// </summary>
    /// <param name="host"> Путь к серверу. </param>
    /// <param name="port"> Порт сервера. </param>
    internal Client(string host, int port)
    {
        _host = host;
        _port = port;
    }

    /// <summary>
    /// Подключение к серверу
    /// </summary>
    async internal void Start()
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(_host, _port);
        var stream = tcpClient.GetStream();

        var response = new List<byte>();
        int bytesRead = 10;

        while (_isRun)
        {
            var word = GetUserInput();

            if (!string.IsNullOrWhiteSpace(word))
            {
                var data = Encoding.UTF8.GetBytes(word + '\n');

                await stream.WriteAsync(data);

                while ((bytesRead = stream.ReadByte()) != '\n')
                {
                    response.Add((byte)bytesRead);
                }
                var responseData = Encoding.UTF8.GetString(response.ToArray());
                var splitResponseData = responseData.Split(" ");
                foreach (var value in splitResponseData)
                {
                    Console.WriteLine($"{value}");
                }
                response.Clear();
            }
            else
            {
                _isRun = false;
            }
        }
        await stream.WriteAsync(Encoding.UTF8.GetBytes("END\n"));
    }

    /// <summary>
    /// Ожидание пользовательского ввода.
    /// </summary>
    /// <returns> Строка введенная пользователем. </returns>
    private string GetUserInput()
    {
        var retString = new StringBuilder();
        while(true)
        {
            var readKeyResult = Console.ReadKey(true);

            if (readKeyResult.Key == ConsoleKey.Escape)
            {
                _isRun = false;
                break;
            }
            else if (readKeyResult.Key == ConsoleKey.Enter)
            {
                break;
            }
            else
            {
                retString.Append(readKeyResult.KeyChar);
                Console.Write(readKeyResult.KeyChar);
            }
        }
        return retString.ToString();
    }
}
