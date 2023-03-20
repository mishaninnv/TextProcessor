using TextProcessor.Models;

namespace TextProcessor.DataBase;

/// <summary>
/// Интерфейс описывающий поведение менеджера базы данных.
/// </summary>
internal interface IDbManager
{
    /// <summary>
    /// Запись данных списка в БД. Удаляет ранее записанные данные при их наличии.
    /// </summary>
    /// <param name="wordList"> Список для занесения в БД. </param>
    internal void CreateList(List<WordModel> wordList);

    /// <summary>
    /// Обновить количество упоминаний слова из нового списка, добавить отсутсвующие слова.
    /// </summary>
    /// <param name="wordList"> Список для обновления. </param>
    internal void UpdateList(List<WordModel> wordList);

    /// <summary>
    /// Удаление всех полей БД, сбрасывает значение счетчиков.(AUTOINCREMENT / IDENTITY)
    /// </summary>
    internal void DeleteList();

    /// <summary>
    /// Получение данных из БД по слову(началу слова)
    /// </summary>
    /// <param name="prefix"> Слово(начало слова) для поиска. </param>
    /// <param name="count"> Количество слов в выборке. </param>
    /// <returns> Перечисление найденных слов (в порядке убывания  
    /// частоты их упоминания в словаре). </returns>
    internal IEnumerable<WordModel> GetWordsByPrefix(string prefix, int count=5);
}
