using SQLiteUnity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class databaseTest : MonoBehaviour
{
    string dbPath, dbFileName = "Word.db";
    SQLite<SQLiteTable<SQLiteRow>, SQLiteRow> Database;

    void Start()
    {
        print(Application.dataPath);
        dbPath = Path.Combine(Application.dataPath, "Resources", "Database", dbFileName);
        using (Database = new(dbPath))
        {
            var data = Database.ExecuteQuery("select name from sqlite_master where type='table';");
            print(data);
            var data2 = Database.ExecuteQuery("SELECT * FROM EnglishWordList600 WHERE ID < 10;");
            print(data2.ToString());
        }
        print("Complete Loading Database");
    }
}
