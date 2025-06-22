using SQLiteUnity;
using System.Collections.Generic;
using System.IO;

public class UnlockDao
{
    private SQLite<SQLiteTable<SQLiteRow>, SQLiteRow> database;
    private string[] wordSetList = CONSTANTS.WORDSET;

    public UnlockDao(string dbDirectory)
    {
        string q = "CREATE TABLE 'Temp' ('ID' INTEGER, PRIMARY KEY ('ID'));";
        database = new SQLite<SQLiteTable<SQLiteRow>, SQLiteRow>("WordUnlocked.db", query: q, path: dbDirectory);
    }

    public void AddUnlockID(int index, int id)
    {
        CheckAndCreateUnlockTable(index);
        var data = database.ExecuteQuery($"SELECT ID FROM {wordSetList[index]} WHERE ID = {id}");
        if (data.IsNullOrEmpty())
        {
            database.ExecuteQuery($"INSERT INTO {wordSetList[index]} VALUES({id})");
        }
    }

    public void CheckAndCreateUnlockTable(int index)
    {
        var data = database.ExecuteQuery($"SELECT name FROM sqlite_master WHERE type='table' AND name = '{wordSetList[index]}';");
        if (data.IsNullOrEmpty())
        {
            database.ExecuteQuery($"CREATE TABLE '{wordSetList[index]}' (ID INTEGER, PRIMARY KEY ('ID'));");
        }
    }

    public List<int> GetUnlockIDList(int index)
    {
        CheckAndCreateUnlockTable(index);
        var data = database.ExecuteQuery($"SELECT name FROM sqlite_master WHERE type='table' AND name = '{wordSetList[index]}';");

        if (!data.IsNullOrEmpty())
        {
            data = database.ExecuteQuery($"SELECT ID FROM {wordSetList[index]}");
            List<int> ls = new List<int>();
            foreach (var i in data.Rows)
            {
                ls.Add((int)i["ID"]);
            }
            return ls;
        }
        else
        {
            return new List<int> { };
        }
    }

    public int GetUnlockIDCount(int index)
    {
        return GetUnlockIDList(index).Count;
    }
}
