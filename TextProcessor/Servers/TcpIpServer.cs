using System.Net.Sockets;
using System.Net;
using System.Text;
using TextProcessor.DataBase;
using TextProcessor.Models;

namespace TextProcessor.Servers;

/// <summary>
/// Класс обрабатывающий клиента по заданному порту (TCP/IP v4).
/// </summary>
internal class TcpIpServer : IServer
{
    private readonly IDbManager _managerDb;

    /// <summary>
    /// Класс обрабатывающий клиента по заданному порту (TCP/IP v4).
    /// </summary>
    /// <param name="dbManager"> Менеджер используемой БД. </param>
    internal TcpIpServer(IDbManager dbManager)
    {
        _managerDb = dbManager;
    }

    /// <summary>
    /// Запустить сервер ожидающий подключения клиентов.
    /// </summary>
    /// <param name="settings"> Настройки для запуска сервера. </param>
    async public void Start(ServerSettingsModel settings)
    {
        var tcpListener = new TcpListener(IPAddress.Any, settings.Port);
        try
        {
            tcpListener.Start();

            while (true)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                Task.Run(async () => await ProcessClientAsync(tcpClient));
            }
        }
        finally
        {
            tcpListener.Stop();
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
                var query = _managerDb
                    .GetWords(queryValues[1].ToLower())
                    .Select(x => x.Word)
                    .ToArray();
                result += string.Join(" ", query);
            }
            else
            {
                result += "Некорректный запрос.";
            }

            result += '\n';
            await stream.WriteAsync(Encoding.UTF8.GetBytes(result));
            response.Clear();
        }
        tcpClient.Close();
    }
}