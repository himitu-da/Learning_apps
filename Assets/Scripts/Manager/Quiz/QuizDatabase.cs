using Cysharp.Threading.Tasks;
using SQLiteUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class QuizDatabase : MonoBehaviour
{
    List<List<string>> correctOptionsList = new List<List<string>>();
    List<List<string>> wrongOptionsList = new List<List<string>>();

    private DatabaseManager databaseManager;

    WordSet database = new WordSet();
    FlexibleWordSet flexibleDatabase = new FlexibleWordSet();

    bool isCorrect, isFirst;
    int databaseCount;

    //flexibleDatabaseリストの変更に使う
    public event Action<bool> OnQuizResult;
    int beforeN, beforeIndex;

    /// <summary>
    /// コンストラクタでは、データベースの取得を行う
    /// </summary>
    public QuizDatabase()
    {

    }

    private void Awake()
    {
        databaseManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DatabaseManager>();
        int databaseType = PlayerPrefs.GetInt("Gold", 0);
        SetDatabase(databaseType);

        /*
        if (databaseType == 0) {
            SetDatabase(CONSTANTS.WORDSET[databaseType], 2, 6);
        }
        else
        {
            SetDatabase(CONSTANTS.WORDSET[databaseType], 1, 0);
        }
        */
        /*
        if (databaseType == 0)
        {
            csvFile = Resources.Load("EnglishWordList600") as TextAsset;
            StringReader reader = new StringReader(csvFile.text);
            reader.ReadLine();
            while (reader.Peek() != -1)
            {
                string[] line = reader.ReadLine().Split(',');   //１行ずつ読む
                Word word = new Word(line[2], line[6]);
                database.Add(word);
                flexibleDatabase.Add(word);
            }
        }
        else if (databaseType == 1)
        {
            csvFile = Resources.Load("Gold1000") as TextAsset;
            StringReader reader = new StringReader(csvFile.text);
            reader.ReadLine();
            int s = 0;
            while (reader.Peek() != -1)
            {
                string[] line = reader.ReadLine().Split(',');   //１行ずつ読む 
                Word word = new Word(line[1], line[0]);
                database.Add(word);
                flexibleDatabase.Add(word);
            }
        }
        else
        {
            csvFile = Resources.Load("Gold1000s") as TextAsset;
            StringReader reader = new StringReader(csvFile.text);
            reader.ReadLine();
            while (reader.Peek() != -1)
            {
                string[] line = reader.ReadLine().Split(',');   //１行ずつ読む 
                Word word = new Word(line[1], line[0]);
                database.Add(word);
                flexibleDatabase.Add(word);
            }
        }
        */
        /*
        switch (databaseType)
        {
            case 0:
                SetDatabase("EnglishWordList600", 2, 6);
                break;
            case 1:
                SetDatabase("Gold1000", 1, 0);
                break;
            case 2:
                SetDatabase("Gold1000s", 1, 0);
                break;
            case 3:
                SetDatabase("jukugosmall", 1, 0);
                break;
            case 4:
                SetDatabase("jukugo1000", 1, 0);
                break;
        }*/

        OnQuizResult += (bool t) => isCorrect = t;
        isFirst = true;
        databaseCount = GetDatabaseCount();
    }

    void SetDatabase(int index)
    {
        var data = databaseManager.WordDao.GetSQLiteWordSet(index);
        List<int> unlockWordSetIDList = databaseManager.UnlockDao.GetUnlockIDList(index);
        foreach (var w in data.Rows)
        {
            if (unlockWordSetIDList.Contains((int)w["ID"]))
            {
                Word word = new(w["Vocab"].ToString(), w["Exp"].ToString());
                database.Add(word);
                flexibleDatabase.Add(word);
            } }
        databaseManager.WordDao.GetSQLiteWordSetHideLocked(index);
    }

    /*
    void SetDatabase(string name, int lineIndex1, int lineIndex2)
    {
        TextAsset csvFile = Resources.Load("Database/" + name) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);
        reader.ReadLine();
        while (reader.Peek() != -1)
        {
            string[] line = reader.ReadLine().Split(',');   //１行ずつ読む
            Word word = new Word(line[lineIndex1], line[lineIndex2]);
            database.Add(word);
            flexibleDatabase.Add(word);
        }
    }
    */
    public int GetDatabaseCount()
    {
        return database.Count;
    }

    public void DoChangeIsCorrect(bool isCorrect)
    {
        OnQuizResult(isCorrect);
    }

    List<string> quizSet = new List<string>();

    public (List<List<string>>, List<List<string>>) GetListSet(int correctOptionsCount, int wrongOptionsCount)
    {
        int sortType = PlayerPrefs.GetInt("Sort_Type", 0);

        if(sortType == 2) ChangeFlexibleList();

        correctOptionsList.Clear();
        wrongOptionsList.Clear();

        quizSet.Clear();

        if (database.Count < correctOptionsCount + wrongOptionsCount)
        {
            throw new Exception("Count is more than databaseCount");
        }

        //正解となるものを作成
        while (correctOptionsList.Count < correctOptionsCount)
        {
            int n;
            switch(sortType)
            {
                case 0:
                    n = UnityEngine.Random.Range(0, databaseCount);
                    quizSet = database.Copy(n);
                    break;
                case 1:
                    n = PlayerPrefs.GetInt("Quiz_Count", 0) % databaseCount;
                    quizSet = database.Copy(n);
                    PlayerPrefs.SetInt("Quiz_Count", n + 1);
                    break;
                case 2:
                    (int index1, int index2) = GetNextFlexibleNum();
                    quizSet = flexibleDatabase.Copy(index1, index2);
                    break;
                default:
                    n = 0;
                    break;
            }
            if (!correctOptionsList.Any(k => k.Contains(quizSet[1])))
            {
                quizSet.Insert(0, "Correct");
                correctOptionsList.Add(quizSet);
            }
        }

        //不正解となるものを作成
        while(wrongOptionsList.Count < wrongOptionsCount)
        {
            int n = UnityEngine.Random.Range(0, databaseCount);
            quizSet = database.Copy(n);
            bool check = correctOptionsList.Any(k => k.Contains(quizSet[1]))
                || correctOptionsList.Any(k => k.Contains(quizSet[0]))
                || wrongOptionsList.Any(k => k.Contains(quizSet[1]))
                || wrongOptionsList.Any(k => k.Contains(quizSet[0]));
            if (!check)
            {
                quizSet.Insert(0, "Wrong");
                wrongOptionsList.Add(quizSet);
            }
        }

        return (correctOptionsList, wrongOptionsList);
    }

    void ChangeFlexibleList()
    {
        int delta;

        if (isFirst)
        {
            isFirst = false;
            return;
        }

        if (isCorrect)
            delta = -(int)Math.Log(Math.Max(databaseCount, 6), 6);
        else
            delta = (int)Math.Log(Math.Max(databaseCount, 1), 4) + 1;

        //Debug.Log($"Delta is: {delta}");
        flexibleDatabase.Move(beforeIndex, beforeN, delta);

        /*
        List<int> list =  flexibleDatabase.GetCountList;
        string str = "";
        foreach (int i in list)
            str += i + ",";

        print("Finished to Move from " + beforeIndex + ", " + beforeN);
        print(str);*/
    }

    List<int> ints = new();

    (int, int) GetNextFlexibleNum()
    {
        ints.Clear();
        for (int i = 0; i < flexibleDatabase.Count; i++)
        {
            ints.Add(flexibleDatabase.CountIndex(i) * (int)Mathf.Pow(2, i));
        }
        int num = UnityEngine.Random.Range(1, ints.Sum()), index;
        (num, index) = GetNextIndex(num, ints);
        beforeN = num / (int)Mathf.Pow(2, index);
        beforeIndex = index;
        return (beforeIndex, beforeN);
    }

    static (int, int) GetNextIndex(int num, List<int> nums)
    {
        int index = 0;
        while (num > 0)
        {
            num -= nums[index];
            if (num > 0)
                index++;
        }
        //print("ints sum: " + nums.Sum() + ", index: " + index);
        return (-1 * num, index);
    }
}

class Word
{
    public string From { get;}
    public string To { get;}

    int index1, index2;

    public Word(string from, string to)
    {
        From = from;
        To = to;
    }

    public void SetIndex(int index1, int index2)
    {
        this.index1 = index1;
        this.index2 = index2;
    }
}

class WordSet
{
    List<Word> words;
    //SQLiteTable<SQLiteRow> table;
    //DatabaseManager dbManager;
    //int count;

    public WordSet()
    {
        words = new List<Word>();
    }
    /*
    public void SetWordSet(DatabaseManager databaseManager,int index)
    {
        dbManager = databaseManager;
        table = dbManager.GetSQLiteWordSet(index);
        count = table.Rows.Count;
    }*/

    public void Add(Word word)
    {
        words.Add(word);
    }

    public int Count { get { return words.Count; } }

    public List<string> Copy(int index)
    {
        return new List<string> { words[index].From, words[index].To };
    }
}

class FlexibleWordSet
{
    List<List<Word>> words;
    int maxListIndex = 1;

    public FlexibleWordSet()
    {
        words = new List<List<Word>>
        {
            new List<Word>()
        };
    }

    public void Add(Word word, int index = 0)
    {
        words[index].Add(word);
        //Debug.Log(maxListIndex);
    }
    public List<string> Copy(int index1, int index2)
    {
        return new List<string> { words[index1][index2].From, words[index1][index2].To };
    }

    public void Move(int befIndex1, int befIndex2, int delta)
    {
        // リストサイズの調節
        maxListIndex = (int)Math.Log(Math.Max(words.Count, 1), 1.7f) + 3;

        // beforeIndexとbeforeNの範囲チェック
        if (befIndex1 < 0 || befIndex1 > words.Count - 1 || befIndex2 < 0 || befIndex2 > words[befIndex1].Count - 1)
        {
            throw new IndexOutOfRangeException("Invalid befIndex1 or befIndex2");
        }

        int aftIndex = befIndex1 + delta;

        if (aftIndex >= 0 && aftIndex <= maxListIndex) // 移動先が範囲内に収まるとき
        {
            while (words.Count - 1 < aftIndex)
                words.Add(new List<Word>());
        }
        else if (aftIndex < 0) // 移動先が負となり、負方向へ拡張するとき
        {
            while (aftIndex < 0)
            {
                words.Insert(0, new List<Word>());
                aftIndex++;
                befIndex1++;

                if (words.Count - 1 >= maxListIndex) // 移動先を0に調節する
                {
                    aftIndex = 0;
                    break;
                }
            }
        }
        else if (aftIndex > maxListIndex) // 移動先が範囲を超えたとき
        {
            while (words.Count - 1 < maxListIndex)
                words.Add(new List<Word>());

            aftIndex = maxListIndex;
        }

        Word word = words[befIndex1][befIndex2];
        words[befIndex1].RemoveAt(befIndex2);
        words[aftIndex].Add(word);

        while (words[0].Count == 0)
            words.RemoveAt(0);
        while (words[words.Count - 1].Count == 0)
            words.RemoveAt(words.Count - 1);

        Debug.Log($"Move FlexibleList from {befIndex1} to {aftIndex}");
    }

    public int Count { get { return words.Count; } }

    public int CountIndex(int index)
    {
        return words[index].Count;
    }

    public List<int> GetCountList
    {
        get
        {
            List<int> list = new();
            for(int i = 0; i < words.Count; i++)
                list.Add(words[i].Count);
            return list;
        }
    }
}
