namespace TextProcessorClient.Models;

/// <summary>
/// Модель отвечающая за настройки клиента.
/// </summary>
internal class ClientSettingsModel
{
    internal string Host { get; set; } = "127.0.0.1";
    internal int Port { get; set; } = 5000;
}
