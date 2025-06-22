using SQLiteUnity;
using System.IO;
using UnityEngine;

public class WordDao
{
    private SQLite<SQLiteTable<SQLiteRow>, SQLiteRow> database;

    public WordDao(string dbPath)
    {
        database = new SQLite<SQLiteTable<SQLiteRow>, SQLiteRow>(Path.GetFileName(dbPath), path: Path.GetDirectoryName(dbPath));
    }

    public SQLiteTable<SQLiteRow> DoQuery(string query)
    {
        return database.ExecuteQuery(query);
    }

    public SQLiteTable<SQLiteRow> GetSQLiteWordSet(int index)
    {
        return DoQuery($"SELECT * FROM {CONSTANTS.WORDSET[index]}");
    }

    public SQLiteRow GetSQLiteWord(int index, int id)
    {
        var data = DoQuery($"SELECT * FROM {CONSTANTS.WORDSET[index]} WHERE ID = {id}").Rows[0];
        if (data.IsNullOrEmpty())
        {
            return null;
        }
        else
        {
            return data;
        }
    }

    public SQLiteTable<SQLiteRow> GetSQLiteWordSetWithoutLocked(int index)
    {
        return DoQuery($"SELECT Vocab, Exp FROM {CONSTANTS.WORDSET[index]}");
    }

    public SQLiteTable<SQLiteRow> GetSQLiteWordSetHideLocked(int index)
    {
        return DoQuery($"SELECT Vocab, Exp FROM {CONSTANTS.WORDSET[index]}");
    }

    public int GetAllIDCount(int index)
    {
        var data = DoQuery($"SELECT COUNT(*) AS COUNT FROM {CONSTANTS.WORDSET[index]};");
        if (!data.IsNullOrEmpty())
            return (int)data.Rows[0]["COUNT"];
        else
            return 0;
    }
}
