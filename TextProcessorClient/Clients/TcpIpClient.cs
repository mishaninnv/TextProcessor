using System.Net.Sockets;
using System.Text;
using TextProcessorClient.Logic;
using TextProcessorClient.Models;

namespace TextProcessorClient.Clients;

/// <summary>
/// Класс отвечающий за работу клиента с сервером по протоколу TCP/IP v4.
/// </summary>
internal class TcpIpClient : IClient
{
    private bool _isRun = true;
    private string _host = "127.0.0.1";
    private int _port;

    /// <summary>
    /// Запуск работы клиента.
    /// </summary>
    /// <param name="settings"> Настройки клиента. </param>
    public Task Start(ClientSettingsModel settings) =>
        Task.Run(() =>
        {
            _host = settings.Host;
            _port = settings.Port;
            while (_isRun)
            {
                var word = UserInteraction.GetUserInput();
                SendRequest(word);
            }
        });

    /// <summary>
    /// Отправка запроса на сервер.
    /// </summary>
    /// <param name="word"> Запрашиваемая конструкция. Метод с параметром. </param>
    private async void SendRequest(string word)
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(_host, _port);
        var stream = tcpClient.GetStream();

        var response = new List<byte>();
        var bytesRead = 10;
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
        await stream.WriteAsync(Encoding.UTF8.GetBytes("END\n"));
    }
}
