using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TextProcessor.Models;

namespace TextProcessor.DataBase;

internal class InitializationDb
{
    private string _connectionString;
    private string _databaseName;
    private string _tableName;

    public InitializationDb()
    {
        var confManager = ConfigurationManager.AppSettings;
        _connectionString = confManager?.Get("InitializeConnectionString") ?? string.Empty;
        _databaseName = confManager?.Get("NameDb") ?? string.Empty;
        _tableName = confManager?.Get("TableName") ?? string.Empty;
    }

    public void Initialization()
    {
        var myConn = new SqlConnection(_connectionString);

        var query = $"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{_databaseName}') BEGIN CREATE DATABASE [{_databaseName}] END";
        var query2 = $"USE [{_databaseName}]";
        var query3 = $"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{_tableName}' and xtype='U') " +
                        $"BEGIN CREATE TABLE {_tableName} " +
                            $"(Id INT PRIMARY KEY IDENTITY (1, 1), " +
                            $"{nameof(WordModel.Word)} VARCHAR(15), " +
                            $"{nameof(WordModel.Count)} INT) " +
                        $"END";
        var query4 = $"IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'index1' AND object_id = OBJECT_ID('{_tableName}')) " +
                    $"BEGIN  CREATE UNIQUE INDEX index1 ON dbo.{_tableName} ({nameof(WordModel.Count)} DESC, {nameof(WordModel.Word)} ASC); " +
                    $"END";

        var myCommand = new SqlCommand(query, myConn);
        try
        {
            myConn.Open();
            myCommand.ExecuteNonQuery();
            myCommand.CommandText = query2;
            myCommand.ExecuteNonQuery();
            myCommand.CommandText = query3;
            myCommand.ExecuteNonQuery();
            myCommand.CommandText = query4;
            myCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
        }
    }
}
