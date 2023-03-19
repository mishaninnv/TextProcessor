using TextProcessorClient.Clients;
using TextProcessorClient.Models;

namespace TextProcessorClient.Logic;

/// <summary>
/// Класс отвечающий за запуск клиента.
/// </summary>
internal class ClientsManager
{
    private readonly IClient _client;
    private readonly ClientSettingsModel _settings;

    /// <summary>
    /// Класс отвечающий за запуск клиента.
    /// </summary>
    /// <param name="client"> Клиент для запуска. </param>
    /// <param name="settings"> Параметры для работы клиента. </param>
    internal ClientsManager(IClient client, ClientSettingsModel settings)
    {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Запуск работы клиента.
    /// </summary>
    internal async Task StartClient()
    {
        await _client.Start(_settings);
    }
}
