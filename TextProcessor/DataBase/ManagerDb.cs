using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using TextProcessor.Models;

namespace TextProcessor.DataBase;

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
                $"BEGIN CREATE TABLE {_tableName} " +
                    $"(Id INT PRIMARY KEY IDENTITY (1, 1), " +
                    $"{nameof(WordModel.Word)} VARCHAR(15), " +
                    $"{nameof(WordModel.Count)} INT) " +
                $"END";
            var initIndex = $"IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'index1' AND object_id = OBJECT_ID('{_tableName}')) " +
                        $"BEGIN  CREATE UNIQUE INDEX index1 ON dbo.{_tableName} ({nameof(WordModel.Count)} DESC, {nameof(WordModel.Word)} ASC); " +
                        $"END";
            sqlConnection.Execute(initTable);
            sqlConnection.Execute(initIndex);
        }
    }

    internal void CreateList()
    { 
        
    }

    internal void DeleteList()
    { 
        
    }

    internal void UpdateList()
    { 
        
    }

    internal List<WordModel> GetWords(string prefix)
    {
        return new List<WordModel>();        
    }
}
