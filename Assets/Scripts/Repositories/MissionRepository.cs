using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// 達成項目を保存する部分
/// </summary>
[System.Serializable]
public class MissionRepository
{
    private readonly string path;
    private readonly SaveLoadService saveLoadService;
    private List<Mission> missions;
    public bool isLoadedFirstMissions = false; // directoryからのミッションの読み込み
    public bool isLoadedSecondMissions = false; // 日々のミッションの追加
    public MissionRepository(string p, SaveLoadService sls)
    {
        path = p;
        saveLoadService = sls;
        LoadMissions();
    }

    void LoadMissions()
    {
        if (!File.Exists(path))
        {
            missions = new();
            SaveMissions();
            return;
        }

        string json = saveLoadService.Load(path);
        Debug.Log(json);
        this.missions = MissionListJsonConverter.FromJson(json);
        Debug.Log("missions count: " + missions.Count);
    }

    public void SaveMissions()
    {
        string json = MissionListJsonConverter.ToJson(missions);
        Debug.Log("here is saving data: " + json);
        saveLoadService.Save(path, json);
    }

    async public void AddMission(Mission mission)
    {
        await UniTask.WaitUntil(() => isLoadedFirstMissions);
        missions.Add(mission);
        SaveMissions();
        Debug.Log("missions saved");
    }

    async public UniTask<int> CountMisson()
    {
        await UniTask.WaitUntil(() => isLoadedFirstMissions);
        return missions.Count();
    }

    async public UniTask<int> CountMission<T>() where T : Mission
    {
        await UniTask.WaitUntil(() => isLoadedFirstMissions);
        return missions.Count(m => m is T);
    }

    async public UniTask<int> CountValidMission()
    {
        await UniTask.WaitUntil(() => isLoadedFirstMissions);
        List<Mission> m = await GetValidMissions();
        return m.Count();
    }

    async public UniTask<int> CountValidMission<T>() where T : Mission
    {
        await UniTask.WaitUntil(() => isLoadedFirstMissions);
        List<Mission> m = await GetValidMissions();
        return m.Count(m => m is T);
    }

    async public UniTask<List<Mission>> GetMissions()
    {
        await UniTask.WaitUntil(() => isLoadedFirstMissions);
        return missions;
    }

    async public UniTask<List<T>> GetMissionsByType<T>() where T : Mission
    {
        List<T> specificMissions = new List<T>();
        await UniTask.WaitUntil(() => isLoadedFirstMissions);

        foreach (Mission m in missions)
        {
            if (m is T mm)
            {
                specificMissions.Add(mm);
            }
        }

        return specificMissions;
    }

    async public UniTask<List<Mission>> GetValidMissions(bool isEliminateCompleted = false)
    {
        List<Mission> validMissions = new();
        await UniTask.WaitUntil(() => isLoadedFirstMissions);

        foreach (Mission m in missions)
        {
            if (m.UntilTime > DateTime.Now)
            {
                if (!isEliminateCompleted || !m.IsCompleted)
                    validMissions.Add(m);
            }
        }

        return validMissions;
    }

    async public UniTask<List<T>> GetValidMissions<T>(bool isEliminateCompleted = false) where T : Mission
    {
        List<T> validMissions = new();
        foreach (T m in await GetMissionsByType<T>())
        {
            if (m.UntilTime > DateTime.Now)
            {
                if (!isEliminateCompleted || !m.IsCompleted)
                    validMissions.Add(m);
            }
        }

        return validMissions;
    }

    async public UniTask<Dictionary<string, MissionData>> GetValidMissionDataDictionaryByType(string type)
    {
        Dictionary<string, MissionData> missionDataDictionary = new();
        await UniTask.WaitUntil(() => isLoadedSecondMissions);

        foreach (Mission m in await GetValidMissions())
        {
            if (m.Type == type)
            {
                MissionData missionData = new MissionData(m);
                missionDataDictionary.Add(m.Name, missionData);
                Debug.Log($"There is {m.Name} on missions list.");
            }
        }
        return missionDataDictionary;
    }

    async public void SetIsGetReward(MissionData missionData, bool isGetReward)
    {
        List<Mission> extractMissions = new(); //missions.Where(m => m.ID == missionData.ID && m.Type == missionData.Type);
        await UniTask.WaitUntil(() => isLoadedSecondMissions);

        foreach (Mission m in missions)
        {
            if (missionData.ID == m.ID && missionData.Type == m.Type)
            {
                extractMissions.Add(m);
            }
        }
        if (extractMissions.Count != 1)
        {
            Debug.LogError("Mission items that are not unique are included in the mission list.");
        }
        extractMissions[0].SetIsGetReward(isGetReward);
        SaveMissions();
    }
}
