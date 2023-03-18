using TextProcessorClient.Models;

namespace TextProcessorClient.Clients;

/// <summary>
/// Интерфейс характеризующий поведение клиента.
/// </summary>
internal interface IClient
{
    /// <summary>
    /// Запуск клиента.
    /// </summary>
    /// <param name="host"> Путь к серверу. </param>
    /// <param name="port"> Порт сервера. </param>
    public Task Start(ClientSettingsModel settings);
}
