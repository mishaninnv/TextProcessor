using System.Net.Sockets;
using System.Net;
using System.Text;
using TextProcessor.DataBase;

namespace TextProcessor;

internal class Server
{
    private readonly ManagerDb _managerDb;
    private readonly int _port;

    internal Server(int port)
    {
        _managerDb = new ManagerDb();
        _port = port;
    }

    async internal void Start()
    {
        var tcpListener = new TcpListener(IPAddress.Any, _port);

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

    async Task ProcessClientAsync(TcpClient tcpClient)
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