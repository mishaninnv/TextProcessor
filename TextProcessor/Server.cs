using System.Net.Sockets;
using System.Net;
using System.Text;
using TextProcessor.DataBase;

namespace TextProcessor;

/// <summary>
/// Класс регистрирующий слушателя по заданному порту.
/// </summary>
internal class Server
{
    private readonly ManagerDb _managerDb;
    private readonly int _port;
    private readonly TcpListener _tcpListener;

    /// <summary>
    /// Класс регистрирующий слушателя по заданному порту.
    /// </summary>
    /// <param name="port"> Порт для прослушивания. </param>
    internal Server(int port)
    {
        _managerDb = new ManagerDb();
        _port = port;
        _tcpListener = new TcpListener(IPAddress.Any, _port);
    }

    /// <summary>
    /// Запустить сервер ожидающий подключения клиентов.
    /// </summary>
    async internal void Start()
    {
        try
        {
            _tcpListener.Start();

            while (true)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                Task.Run(async () => await ProcessClientAsync(tcpClient));
            }
        }
        finally
        {
            _tcpListener.Stop();
        }
    }

    /// <summary>
    /// Метод обработки запроса клиента.
    /// </summary>
    /// <param name="tcpClient"> Данные о подключении клиента. </param>
    private async Task ProcessClientAsync(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        var response = new List<byte>();
        var bytesRead = 10;
        while (true)
        {
            while ((bytesRead = stream.ReadByte()) != '\n')
            {
                response.Add((byte)bytesRead);
            }
            var responseQuery = Encoding.UTF8.GetString(response.ToArray());

            if (responseQuery == "END") break;

            var queryValues = responseQuery.Split(' ');
            var result = string.Empty;
            if (queryValues.Length == 2 
                && queryValues[0].ToLower().Equals("get"))
            {
                result += _managerDb
                    .GetWords(queryValues[1])
                    .Select(x => x.Word)
                    .ToArray().ToString();
            }
            else
            {
                result += "Запрашиваемый метод отсутсвует.";
            } 

            result += '\n';
            await stream.WriteAsync(Encoding.UTF8.GetBytes(result));
            response.Clear();
        }
        tcpClient.Close();
    }
}