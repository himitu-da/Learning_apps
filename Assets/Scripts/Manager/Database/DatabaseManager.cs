using SQLiteUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class DatabaseManager : MonoBehaviour
{
    public WordDao WordDao { get; private set; }
    public UnlockDao UnlockDao { get; private set; }

    void Awake()
    {
        string dbPathDirectory = Path.Combine(Application.persistentDataPath, "Database");

        if (!Directory.Exists(dbPathDirectory))
        {
            Directory.CreateDirectory(dbPathDirectory);
        }

        // Resourcesフォルダからデータベースファイルをロードし、永続パスにコピー
        TextAsset textAsset = Resources.Load<TextAsset>("Database/Word");
        byte[] databaseAsset = textAsset.bytes;
        string dbPath = Path.Combine(dbPathDirectory, "Word.db");
        File.WriteAllBytes(dbPath, databaseAsset);

        // DAOの初期化
        WordDao = new WordDao(dbPath);
        UnlockDao = new UnlockDao(dbPathDirectory);
    }

    /*
    public TextAsset GetWordSetByIndex(int index)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Database/" + wordSetList[index]);
        print("loaded " + wordSetList[index]);
        if (textAsset == null)
            print("textasset is null");
        return textAsset;
    }

    public TextAsset GetWordSetByName(string name)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Database/" + wordSetList.First(n => n == name));
        return textAsset;
    }

    public List<List<string>> ParseToWordSet(TextAsset textAsset, params int[] linesIndex)
    {
        List<List<string>> lists = new();
        using (StringReader stringReader = new(textAsset.text))
        {
            List<string> words = new List<string>();
            string line;
            stringReader.ReadLine(); // 一行目は無視
            while ((line = stringReader.ReadLine()) != null)
            {
                words.Clear();
                string[] strings = line.Split(",");
                foreach (int lineIndex in linesIndex)
                {
                    words.Add(strings[lineIndex]);
                }
                lists.Add(words.ToList());
            }
        }
        return lists;
    }
    */
}
