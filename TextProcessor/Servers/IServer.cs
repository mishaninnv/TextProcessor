using TextProcessor.Models;

namespace TextProcessor.Servers;

/// <summary>
/// Интерфейс характеризующий поведение сервера.
/// </summary>
internal interface IServer
{
    /// <summary>
    /// Запуск сервера.
    /// </summary>
    /// <param name="settings"> Настройки сервера. </param>
    internal void Start(ServerSettingsModel settings);
}
