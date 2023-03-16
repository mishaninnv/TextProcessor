using Dapper;
using System.Data.SqlClient;
using TextProcessor.Models;

namespace TextProcessor.DataBase;

/// <summary>
/// Класс содержащий методы для работы с БД.
/// </summary>
internal class ManagerDb
{
    private readonly string _host;
    private readonly string _initDatabase = "master";
    private readonly string _password = "89d5am!#IH";
    private readonly string _connectDatabase = "TextProcessor";
    private readonly string _tableName = "WordList";
    private readonly string _connectionString;

    /// <summary>
    /// Класс содержащий методы для работы с БД.
    /// </summary>
    /// <param name="host"> Путь к базе данных. </param>
    public ManagerDb(string host = "localhost")
    {
        _host = host;
        _connectionString = $"Server={_host};" +
                            $"database={_connectDatabase};" +
                            $"User Id=sa;" +
                            $"Password={_password};";
        InitDb();
    }

    /// <summary>
    /// Запись данных списка в БД. Удаляет ранее записанные данные при их наличии.
    /// </summary>
    /// <param name="wordList"> Список для занесения в БД. </param>
    internal void CreateList(List<WordModel> wordList)
    {
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
    internal void UpdateList(List<WordModel> wordList)
    {
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
    /// Удаление всех полей БД, сбрасывает значение счетчиков.(AUTOINCREMENT / IDENTITY)
    /// </summary>
    internal void DeleteList()
    {
        using var sqlConnection = new SqlConnection(_connectionString);
        sqlConnection.Execute($"TRUNCATE TABLE {_tableName}");
    }

    /// <summary>
    /// Получение данных из БД по слову(началу слова)
    /// </summary>
    /// <param name="prefix"> Слово(начало слова) для поиска. </param>
    /// <returns> Перечисление до 5 найденных слов (в порядке убывания их частоты упоминания в словаре). </returns>
    internal IEnumerable<WordModel> GetWords(string prefix)
    {
        using var sqlConnection = new SqlConnection(_connectionString);
        var query = $"SELECT TOP(5) {nameof(WordModel.Word)} FROM {_tableName} " +
                    $"WHERE {nameof(WordModel.Word)} LIKE N'{prefix}%'";
        return sqlConnection.Query<WordModel>(query);
    }

    /// <summary>
    /// Инициализирует базу данных с таблицей если таковой нет.
    /// </summary>
    private void InitDb()
    {
        var initConnectionString = $"Server={_host};" +
                                   $"database={_initDatabase};" +
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
                                $"{nameof(WordModel.Word)} NVARCHAR(15), " +
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