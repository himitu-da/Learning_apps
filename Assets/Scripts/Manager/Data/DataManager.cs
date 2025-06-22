using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using System;
using UnityEngine.UIElements;
using System.Data;
using System.Linq;
using UnityEditor;
using Firebase.Firestore;
using Cysharp.Threading.Tasks.Triggers;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
using Google.MiniJSON;
using TMPro;
using Cysharp.Threading.Tasks;

public class DataManager : MonoBehaviour
{
    static string root_path = "";
    public string format = CONSTANTSDATE.FORMAT;
    public string date_format = CONSTANTSDATE.DAYFORMAT;

    static string[] quizType = new string[] { "English_Word_Quiz", "Endress_Quiz", "Speed_Up_Quiz"};
    static string[] quizStatus = new string[] { "Ever_Correct_Count", "Ever_Quiz_Count" };
    public ResourceManager res;
    public AchieveManager achi;
    private SaveLoadService saveLoadService;
    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        root_path = Application.persistentDataPath;
        saveLoadService = new SaveLoadService();
        Debug.Log("First catching root path is:" + root_path);
        if (!Directory.Exists(root_path))
        {
            Directory.CreateDirectory(root_path);
            Debug.Log("Directory created at: " + root_path);
        }
        create();
        convert();
        GameObject g = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG);
        res = g.AddComponent<ResourceManager>();
        achi = g.AddComponent<AchieveManager>();
        IsInitialized = true;
    }

    #region Save Methods
    public void SaveEveryPlayData(Every_Play_Data data)
    {
        string json = JsonUtility.ToJson(data);
        string full_path = Path.Combine(root_path, "every_play_data", data.uuid.ToString() + ".json");
        saveLoadService.Save(full_path, json);
    }

    public void SaveDayPlayData(Day_Play_Data data)
    {
        DateTime datetime = DateTime.ParseExact(data.date, date_format, null);
        string json = JsonUtility.ToJson(data);
        string full_path = Path.Combine(root_path, "play_data", datetime.Year.ToString(), datetime.Month.ToString(), datetime.Day + ".json");
        saveLoadService.Save(full_path, json);
    }

    public void SaveBasicData(Basic_Data data)
    {
        string json = JsonUtility.ToJson(data);
        string full_path = Path.Combine(root_path, "basic_data", data.type + ".json");
        saveLoadService.Save(full_path, json);
    }
    #endregion

    public Day_Play_Data load_day_play_data(string date, bool isMakeFile = true) // yyyy-MM-dd
    {
        DateTime datetime = DateTime.ParseExact(date, date_format, null);
        string full_path = Path.Combine(root_path, "play_data", datetime.Year.ToString(), datetime.Month.ToString(), datetime.Day.ToString() + ".json");
        Debug.Log("start loading day_play_data at:" + full_path + " The loading date is " + date);

        if (!File.Exists(full_path) && isMakeFile) //ファイルが存在しない場合、ファイルを作る
        {
            Directory.CreateDirectory(Path.Combine(root_path, "play_data", datetime.Year.ToString(), datetime.Month.ToString()));
            string save_json = JsonUtility.ToJson(new Day_Play_Data { date = datetime.ToString(date_format) });
            saveLoadService.Save(full_path, save_json);
            Debug.Log("Created_Day_Play_Data at:" + full_path);
        }

        Day_Play_Data data;

        if (File.Exists(full_path))
        {
            string json = saveLoadService.Load(full_path);
            data = JsonUtility.FromJson<Day_Play_Data>(json);
            Debug.Log("Finished loading day_play_data");
            return data;
        }
        else
        {
            Debug.Log("There is no such file: " + full_path);
            return new Day_Play_Data { date = datetime.ToString(date_format) };
        }
        /*
        try
        {
            string json = AES_load(full_path);
            data = JsonUtility.FromJson<Day_Play_Data>(json);
            Debug.Log("Finished loading day_play_data");
            return data;
        }
        catch (Exception e)
        {
            Debug.Log("There is no such file: " + full_path);
            Debug.LogError (e.Message);
            return new Day_Play_Data { date = datetime.ToString(date_format) } ;
        }*/
    }

    public Day_Play_Data load_today_play_data() // yyyy-MM-dd
    {
        DateTime datetime = DateTime.Now;
        return load_day_play_data(datetime.ToString(date_format));
    }

    public Basic_Data load_basic_data(string type)
    {
        string full_path = Path.Combine(root_path, "basic_data", type + ".json");

        if (!File.Exists(full_path)) //ファイルが存在しない場合、ファイルを作る
        {
            string save_json = JsonUtility.ToJson(new Basic_Data { type = type });
            saveLoadService.Save(full_path, save_json);
            Debug.Log("Created_Day_Play_Data at:" + full_path);
        }

        try
        {
            string json = saveLoadService.Load(full_path);
            Basic_Data basic_data = JsonUtility.FromJson<Basic_Data>(json);
            return basic_data;
        }
        catch (Exception e)
        {
            Debug.Log("There is no such file: " + full_path);
            Debug.LogError(e.Message);
            return new Basic_Data { type = type };
        }
    }

    void convert()
    {
        string[] files = Directory.GetFiles(Path.Combine(root_path, "every_play_data"), "*.json");
        DateTime datetime;
        int year;
        int month;
        int day;

        foreach (string file in files)
        {
            string json = saveLoadService.Load(file);
            Every_Play_Data data = JsonUtility.FromJson<Every_Play_Data>(json);
            datetime = DateTime.ParseExact(data.date, format, null);
            year = datetime.Year;
            month = datetime.Month;
            day = datetime.Day;
            string path = Path.Combine(root_path, "play_data", year.ToString(), month.ToString());

            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Day_Play_Data day_play_data = load_day_play_data(datetime.ToString(date_format));
            day_play_data.add(data);
            SaveDayPlayData(day_play_data);
            File.Delete(file);
        }
    }

    void DANGER_RESET()
    {
        MasterData master = new MasterData();
        master.root_path = root_path;
        master.add(new MasterDataEntry { data = "play_data", path = "play_data" });
        master.add(new MasterDataEntry { data = "every_play_data", path = "every_play_data" });
        master.add(new MasterDataEntry { data = "basic_data", path = "basic_data" });
        
        string json = JsonUtility.ToJson(master);
        string full_path = Path.Combine(root_path, "master.json");
        saveLoadService.Save(full_path, json);
    }

    void create()
    {
        string full_path = Path.Combine(Application.persistentDataPath, "master.json");

        Boolean check = PlayerPrefs.HasKey("Endress_Question_Ever_Quiz_Count") || PlayerPrefs.HasKey("English_Word_Quiz_Ever_Quiz_Count") || PlayerPrefs.HasKey("Speed_Up_Quiz_Ever_Quiz_Count");

        if (!File.Exists(full_path))
        {
            DANGER_RESET();

            if (check && PlayerPrefs.GetInt("End_Moving") != 1)
            {
                DateTime start_date = DateTime.ParseExact("2024-05-01", "yyyy-MM-dd", null); //初日
                int days_difference = (DateTime.Now.Date - start_date).Days + 1;

                print(days_difference);

                for (int i = 0; i < days_difference; i++)
                {
                    DateTime check_date = start_date.AddDays(i);
                    int en_score = PlayerPrefs.GetInt("English_Question_Day_Score_" + check_date.ToString("yyyy_MM_dd"));
                    int en_count = PlayerPrefs.GetInt("English_Question_Day_Quiz_Count_" + check_date.ToString("yyyy_MM_dd"));

                    int eq_score = PlayerPrefs.GetInt("Endress_Question_Day_Score_" + check_date.ToString("yyyy_MM_dd"));
                    int eq_count = PlayerPrefs.GetInt("Endress_Question_Day_Quiz_Count_" + check_date.ToString("yyyy_MM_dd"));

                    int sp_score = PlayerPrefs.GetInt("Speed_Up_Quiz_Day_Score_" + check_date.ToString("yyyy_MM_dd"));
                    int sp_count = PlayerPrefs.GetInt("Speed_Up_Quiz_Day_Quiz_Count_" + check_date.ToString("yyyy_MM_dd"));

                    string date_str = check_date.ToString("yyyy-MM-dd");

                    Every_Play_Data a = new Every_Play_Data
                    {
                        date = check_date.ToString(format),
                        type = "English_Word_Quiz",
                        quiz_count = en_count,
                        correct_count = en_score
                    };
                    SaveEveryPlayData(a);

                    Every_Play_Data b = new Every_Play_Data
                    {
                        date = check_date.ToString(format),
                        type = "Endress_Quiz",
                        quiz_count = eq_count,
                        correct_count = eq_score
                    };
                    SaveEveryPlayData(b);

                    Every_Play_Data c = new Every_Play_Data
                    {
                        date = check_date.ToString(format),
                        type = "Speed_Up_Quiz",
                        quiz_count = sp_count,
                        correct_count = sp_score
                    };
                    SaveEveryPlayData(c);
                    /*
                    player_status.daily_status_dic[date_str] = new Daily_Status { date = date_str };

                    player_status.daily_status_dic[date_str].every_status.Add(new Every_Status
                    {
                        quiz_type = "Endress_Quiz",
                        correct_count = eq_score,
                        quiz_count = eq_count
                    });
                    player_status.daily_status_dic[date_str].every_status.Add(new Every_Status
                    {
                        quiz_type = "English_Word_Quiz",
                        correct_count = en_score,
                        quiz_count = en_count
                    });
                    player_status.daily_status_dic[date_str].every_status.Add(new Every_Status
                    {
                        quiz_type = "Speed_Up_Quiz",
                        correct_count = sp_score,
                        quiz_count = sp_count
                    });*/
                }

                int ew_ever_score = PlayerPrefs.GetInt("Endress_Question_Ever_Score");
                int ew_ever_count = PlayerPrefs.GetInt("Endress_Question_Ever_Quiz_Count");
                Basic_Data d = new Basic_Data { type = "Endress_Quiz" };
                d.add(new Every_Basic_Data { name = "Ever_Correct_Count", status = ew_ever_score });
                d.add(new Every_Basic_Data { name = "Ever_Quiz_Count", status = ew_ever_count });
                SaveBasicData(d);

                /*
                player_status.basic_status_dic.Add("Endress_Quiz", new Basic_Status());
                player_status.basic_status_dic["Endress_Quiz"].every_status.Add(new Every_Basic_Status { name = "Ever_Quiz_Count", status = ew_ever_count });
                player_status.basic_status_dic["Endress_Quiz"].every_status.Add(new Every_Basic_Status { name = "Ever_Correct_Count", status = ew_ever_score });
                */

                int en_ever_score = PlayerPrefs.GetInt("English_Word_Quiz_Ever_Score");
                int en_ever_count = PlayerPrefs.GetInt("English_Word_Quiz_Ever_Quiz_Count");
                Basic_Data e = new Basic_Data { type = "English_Word_Quiz" };
                e.add(new Every_Basic_Data { name = "Ever_Correct_Count", status = en_ever_score });
                e.add(new Every_Basic_Data { name = "Ever_Quiz_Count", status = en_ever_count });
                SaveBasicData(e);

                /*
                player_status.basic_status_dic.Add("English_Word_Quiz", new Basic_Status());
                player_status.basic_status_dic["English_Word_Quiz"].every_status.Add(new Every_Basic_Status { name = "Ever_Quiz_Count", status = en_ever_count });
                player_status.basic_status_dic["English_Word_Quiz"].every_status.Add(new Every_Basic_Status { name = "Ever_Correct_Count", status = en_ever_score });
                */

                int sp_ever_score = PlayerPrefs.GetInt("Speed_Up_Quiz_Ever_Score");
                int sp_ever_count = PlayerPrefs.GetInt("Speed_Up_Quiz_Ever_Quiz_Count");
                int sp_level = PlayerPrefs.GetInt("Level");
                int sp_max_level = PlayerPrefs.GetInt("Max_Level");
                Basic_Data f = new Basic_Data { type = "Speed_Up_Quiz" };
                f.add(new Every_Basic_Data { name = "Ever_Correct_Count", status = sp_ever_score });
                f.add(new Every_Basic_Data { name = "Ever_Quiz_Count", status = sp_ever_count });
                f.add(new Every_Basic_Data { name = "Level", status = sp_level });
                f.add(new Every_Basic_Data { name = "Max_Level", status = sp_max_level });
                SaveBasicData(f);
                PlayerPrefs.SetInt("End_Moving", 1);

                /*
                player_status.basic_status_dic.Add("Speed_Up_Quiz", new Basic_Status());
                player_status.basic_status_dic["Speed_Up_Quiz"].every_status.Add(new Every_Basic_Status { name = "Ever_Quiz_Count", status = sp_ever_count });
                player_status.basic_status_dic["Speed_Up_Quiz"].every_status.Add(new Every_Basic_Status { name = "Ever_Correct_Count", status = sp_ever_score });
                player_status.basic_status_dic["Speed_Up_Quiz"].every_status.Add(new Every_Basic_Status { name = "Level", status = sp_level });
                player_status.basic_status_dic["Speed_Up_Quiz"].every_status.Add(new Every_Basic_Status { name = "Max_Level", status = sp_max_level });

                Wrapped_List list = to_list(player_status);
                */
            }
            else
            {
                foreach (string type in quizType)
                {
                    Every_Play_Data a = new Every_Play_Data { type = type };
                    SaveEveryPlayData(a);
                }
            }
        }

        foreach (string type in quizType)
        {
            if (!File.Exists(Path.Combine(root_path, "basic_data", type + ".json")))
            {
                Basic_Data b = load_basic_data(type);
                foreach (string name in quizStatus)
                {
                    if (!b.Exist(name))
                        b.add(new Every_Basic_Data { name = name, status = 0 });
                }
                SaveBasicData(b);
            }
        }
    }

    public MasterData LoadMaster()
    {
        string fullPath = Path.Combine(root_path, "master.json");
        string json = saveLoadService.Load(fullPath);
        MasterData master = JsonUtility.FromJson<MasterData>(json);
        return master;
    }

}
