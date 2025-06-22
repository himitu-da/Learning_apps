using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using SQLiteUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static AchieveManager;
using static DataManager;

/// <summary>
/// 派生クラスの追加あるいはプロパティの変更等をしたときは、適宜MissionListJsonConverterを変更する必要がある。
/// </summary>
public class AchieveManager : MonoBehaviour
{
    DataManager dataManager;
    private SaveLoadService saveLoadService;
    public MissionRepository missionRepository;
    string rootPath, directoryPath, filePath;
    void Awake()
    {
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();
        saveLoadService = new SaveLoadService();

        rootPath = Application.persistentDataPath;
        directoryPath = Path.Combine(rootPath, "Mission");
        filePath = Path.Combine(directoryPath, "MissionMaster.json");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        missionRepository = new(filePath, saveLoadService);
        missionRepository.isLoadedFirstMissions = true;
    }

    async private void Start()
    {
        DateTime today = DateTime.Today;

        if (await missionRepository.CountValidMission<DailyBonusMission>() == 0) // デイリーボーナスを自動追加
        {
            DateTime nowTime = DateTime.Now;
            DateTime untilTime = nowTime.Date.AddDays(1).AddMilliseconds(-1);
            string id = $"{await missionRepository.CountMission<DailyBonusMission>() + 1}";
            string name = CONSTANTSMISSION.NAME[0, 0];
            missionRepository.AddMission(new DailyBonusMission(id, name, 10, 10, nowTime, untilTime));
            Debug.Log("Add Daily Bonus Mission");
        }

        missionRepository.isLoadedSecondMissions = true;
    }

    /// <summary>
    /// 有効なデイリーログインボーナスの進捗を追加する。
    /// </summary>
    /// <param name="num"></param>
    async public void DailyBonusAddProgress(int num)
    {
        List<DailyBonusMission> lists = await missionRepository.GetValidMissions<DailyBonusMission>();
        if (lists.Count > 0)
        {
            foreach (DailyBonusMission m in lists)
            {
                m.UpdateProgress(num);
            }
        }
        missionRepository.SaveMissions();
    }
}

public struct MissionData
{
    public string ID;
    public string Type;
    public string Name;
    public string Description;
    public DateTime CreatedTime;
    public DateTime UntilTime;
    public int Progress;
    public int Goal;
    public bool IsCompleted;
    public bool IsGetReward;

    public MissionData(Mission m)
    {
        ID = m.ID;
        Type = m.Type;
        Name = m.Name;
        Description = m.Description;
        CreatedTime = m.CreatedTime;
        UntilTime = m.UntilTime;
        Progress = m.Progress;
        Goal = m.Goal;
        IsCompleted = m.IsCompleted;
        IsGetReward = m.IsGetReward;
    }
}
