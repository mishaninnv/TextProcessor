using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using TextProcessor.Models;

namespace TextProcessor.DataBase;

/// <summary>
/// Класс содержащий методы для работы с БД.
/// </summary>
internal class ManagerDb
{
    private string _connectionString;
    private string _initConnectionString;
    private string _databaseName;
    private string _tableName;

    public ManagerDb()
    {
        var confManager = ConfigurationManager.AppSettings;
        _connectionString = confManager?.Get("ConnectionString") ?? string.Empty;
        _initConnectionString = confManager?.Get("InitializeConnectionString") ?? string.Empty;
        _databaseName = confManager?.Get("NameDb") ?? string.Empty;
        _tableName = confManager?.Get("TableName") ?? string.Empty;
    }

    /// <summary>
    /// Инициализирует базу данных с таблицей если таковой нет.
    /// </summary>
    internal void InitDb()
    {
        using (var sqlConnection = new SqlConnection(_initConnectionString))
        {
            var initDb = $"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{_databaseName}') BEGIN CREATE DATABASE [{_databaseName}] END";
            sqlConnection.Execute(initDb);
        }

        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            var initTable = $"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{_tableName}' and xtype='U') " +
                            $"BEGIN " +
                            $"CREATE TABLE {_tableName} " +
                                $"(Id INT PRIMARY KEY IDENTITY (1, 1), " +
                                $"{nameof(WordModel.Word)} NVARCHAR(15), " +
                                $"{nameof(WordModel.Count)} INT) " +
                            $"END";
            var initIndex = $"IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'index1' AND object_id = OBJECT_ID('{_tableName}')) " +
                            $"BEGIN " +
                            $"CREATE UNIQUE INDEX index1 ON dbo.{_tableName} ({nameof(WordModel.Count)} DESC, {nameof(WordModel.Word)} ASC); " +
                            $"END";
            sqlConnection.Execute(initTable);
            sqlConnection.Execute(initIndex);
        }
    }

    /// <summary>
    /// Запись данных списка в БД. Удаляет ранее записанные данные при их наличии.
    /// </summary>
    /// <param name="wordList"> Список для занесения в БД. </param>
    internal void CreateList(List<WordModel> wordList)
    {
        DeleteList();
        var finishWordList = wordList.Where(x => x.Count > 2).ToList();
        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            sqlConnection.Execute($"INSERT INTO {_tableName} ({nameof(WordModel.Word)}, {nameof(WordModel.Count)}) VALUES(@{nameof(WordModel.Word)}, @{nameof(WordModel.Count)})", finishWordList);
        }
    }

    /// <summary>
    /// Обновить имеющийся список данными из нового списка. При наличии данных увеличивает вес слова, при отсутсвии добавляет.
    /// </summary>
    /// <param name="wordList"> Список для обновления. </param>
    internal void UpdateList(List<WordModel> wordList)
    {
        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            sqlConnection.Execute($"BEGIN TRAN;" +
                                  $"UPDATE {_tableName} SET {nameof(WordModel.Count)} = @{nameof(WordModel.Count)} WHERE {nameof(WordModel.Word)} = @{nameof(WordModel.Word)}; " +
                                  $"BEGIN " +
                                    $"IF NOT EXISTS (SELECT * FROM {_tableName} WHERE {nameof(WordModel.Word)} = @{nameof(WordModel.Word)}) " +
                                    $"BEGIN " +
                                    $"INSERT INTO {_tableName} ({nameof(WordModel.Word)}, {nameof(WordModel.Count)}) VALUES(@{nameof(WordModel.Word)}, @{nameof(WordModel.Count)})" +
                                    $"END " +
                                  $"END " +
                                  $"DELETE FROM {_tableName} WHERE {nameof(WordModel.Count)} < 3;" +
                                  $"COMMIT TRAN;", wordList);
        }
    }

    /// <summary>
    /// Удаление всех полей БД, сбрасывает значение счетчиков.(AUTOINCREMENT / IDENTITY)
    /// </summary>
    internal void DeleteList()
    {
        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            sqlConnection.Execute($"TRUNCATE TABLE {_tableName}");
        }
    }

    /// <summary>
    /// Получение данных из БД по слову(началу слова)
    /// </summary>
    /// <param name="prefix"> Слово(начало слова) для поиска. </param>
    /// <returns> Перечисление до 5 найденных слов (в порядке убывания их частоты упоминания в словаре). </returns>
    internal IEnumerable<WordModel> GetWords(string prefix)
    {
        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            return sqlConnection.Query<WordModel>($"SELECT TOP(5) {nameof(WordModel.Word)} FROM {_tableName} WHERE {nameof(WordModel.Word)} LIKE N'{prefix}%'");
        }     
    }
}
