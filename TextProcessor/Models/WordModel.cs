namespace TextProcessor.Models;

/// <summary>
/// Класс описывающий модель слова
/// </summary>
internal class WordModel
{
    /// <summary>
    /// Идентификатор для БД
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Слово из списка
    /// </summary>
    public string? Word { get; set; }

    /// <summary>
    /// Количество упоминаний слова в списке
    /// </summary>
    public int Count { get; set; }
}
