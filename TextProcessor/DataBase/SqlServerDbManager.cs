using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using TextProcessor.Models;

namespace TextProcessor.DataBase;

/// <summary>
/// Класс содержащий методы для работы с БД.
/// </summary>
internal class SqlServerDbManager : IDbManager
{
    private readonly string _password;
    private readonly string _connectDatabase;
    private readonly string _tableName;
    private readonly string _connectionString;

    /// <summary>
    /// Класс содержащий методы для работы с БД.
    /// </summary>
    /// <param name="host"> Путь к базе данных. </param>
    internal SqlServerDbManager(string host = "localhost")
    {
        _password = ConfigurationManager.AppSettings["Password"] ?? "89d5am!#IH";
        _connectDatabase = ConfigurationManager.AppSettings["NameDb"] ?? "TextProcessor";
        _tableName = ConfigurationManager.AppSettings["TableName"] ?? "WordList";

        _connectionString = $"Server={host};" +
                            $"database={_connectDatabase};" +
                            $"User Id=sa;" +
                            $"Password={_password};";
        InitDb(host);
    }

    /// <summary>
    /// Запись данных списка в БД. Удаляет ранее записанные данные при их наличии.
    /// </summary>
    /// <param name="wordList"> Список для занесения в БД. </param>
    public void CreateList(List<WordModel> wordList)
    {
        if (wordList.Count == 0)
        {
            return;
        }

        DeleteList();
        var finishWordList = wordList.Where(x => x.Count > 2).ToList();
        using var sqlConnection = new SqlConnection(_connectionString);
        var query = $"INSERT INTO {_tableName} ({nameof(WordModel.Word)}, {nameof(WordModel.Count)}) " +
                    $"VALUES(@{nameof(WordModel.Word)}, @{nameof(WordModel.Count)})";
        sqlConnection.Execute(query, finishWordList);
    }

    /// <summary>
    /// Обновить количество упоминаний слова из нового списка, добавить отсутсвующие слова.
    /// </summary>
    /// <param name="wordList"> Список для обновления. </param>
    public void UpdateList(List<WordModel> wordList)
    {
        if (wordList.Count == 0)
        {
            return;
        }

        using var sqlConnection = new SqlConnection(_connectionString);
        var query = $"BEGIN TRAN;" +
                    $"UPDATE {_tableName} SET {nameof(WordModel.Count)} = @{nameof(WordModel.Count)} " +
                    $"WHERE {nameof(WordModel.Word)} = @{nameof(WordModel.Word)}; " +
                    $"BEGIN " +
                        $"IF NOT EXISTS " +
                        $"(SELECT * FROM {_tableName} " +
                        $"WHERE {nameof(WordModel.Word)} = @{nameof(WordModel.Word)}) " +
                        $"BEGIN " +
                            $"INSERT INTO {_tableName} ({nameof(WordModel.Word)}, {nameof(WordModel.Count)}) " +
                            $"VALUES(@{nameof(WordModel.Word)}, @{nameof(WordModel.Count)})" +
                        $"END " +
                    $"END " +
                    $"DELETE FROM {_tableName} " +
                    $"WHERE {nameof(WordModel.Count)} < 3;" +
                    $"COMMIT TRAN;";
        sqlConnection.Execute(query, wordList);
    }

    /// <summary>
    /// Удаление всех полей таблицы, сбрасывает значение счетчиков.(AUTOINCREMENT / IDENTITY)
    /// </summary>
    public void DeleteList()
    {
        using var sqlConnection = new SqlConnection(_connectionString);
        sqlConnection.Execute($"TRUNCATE TABLE {_tableName}");
    }

    /// <summary>
    /// Получение данных из БД по слову(началу слова)
    /// </summary>
    /// <param name="prefix"> Слово(начало слова) для поиска. </param>
    /// <param name="count"> Количество слов в выборке. </param>
    /// <returns> Перечисление найденных слов (в порядке убывания 
    /// частоты их упоминания в словаре). </returns>
    public IEnumerable<WordModel> GetWordsByPrefix(string prefix, int count = 5)
    {
        using var sqlConnection = new SqlConnection(_connectionString);
        var query = $"SELECT TOP({count}) {nameof(WordModel.Word)} FROM {_tableName} " +
                    $"WHERE {nameof(WordModel.Word)} LIKE N'{prefix}%'";
        return sqlConnection.Query<WordModel>(query);
    }

    /// <summary>
    /// Инициализирует базу данных с таблицей если таких нет.
    /// </summary>
    private void InitDb(string host)
    {
        var initConnectionString = $"Server={host};" +
                                   $"database=master;" +
                                   $"User Id=sa;" +
                                   $"Password={_password};";
        using (var sqlConnection = new SqlConnection(initConnectionString))
        {
            var initDb = $"IF NOT EXISTS" +
                            $"(SELECT * FROM sys.databases " +
                            $"WHERE name = '{_connectDatabase}') " +
                         $"BEGIN " +
                         $"CREATE DATABASE [{_connectDatabase}] " +
                         $"END";
            sqlConnection.Execute(initDb);
        }

        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            var initTable = $"IF NOT EXISTS " +
                                $"(SELECT * FROM sysobjects " +
                                $"WHERE name='{_tableName}' and xtype='U') " +
                            $"BEGIN " +
                            $"CREATE TABLE {_tableName} " +
                                $"(Id INT PRIMARY KEY IDENTITY (1, 1), " +
                                $"{nameof(WordModel.Word)} NVARCHAR(16), " +
                                $"{nameof(WordModel.Count)} INT) " +
                            $"END";
            sqlConnection.Execute(initTable);

            var initIndex = $"IF NOT EXISTS" +
                                $"(SELECT * FROM sys.indexes " +
                                $"WHERE name = 'index1' " +
                                $"AND object_id = OBJECT_ID('{_tableName}')) " +
                            $"BEGIN " +
                            $"CREATE UNIQUE INDEX index1 ON dbo.{_tableName} " +
                            $"({nameof(WordModel.Count)} DESC, {nameof(WordModel.Word)} ASC); " +
                            $"END";

            sqlConnection.Execute(initIndex);
        }
    }
}