// シリアライズ可能な Mission クラス
using static AchieveManager;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

[Serializable]
public class SerializableMission
{
    public string missionType;
    public string id;
    public string type;
    public string name;
    public string description;
    public string createdTime;
    public string untilTime;
    public int progress;
    public int goal;
    public bool isCompleted;
    public bool isGetReward;
    public int rewardFaith;

    public SerializableMission(Mission mission)
    {
        missionType = mission.GetType().Name;
        id = mission.ID;
        type = mission.Type;
        name = mission.Name;
        description = mission.Description;
        createdTime = mission.CreatedTime.ToString(CONSTANTSDATE.FORMAT);
        untilTime = mission.UntilTime.ToString(CONSTANTSDATE.FORMAT);
        progress = mission.Progress;
        goal = mission.Goal;
        isCompleted = mission.IsCompleted;
        isGetReward = mission.IsGetReward;

        if (mission is DailyBonusMission dailyBonus)
        {
            rewardFaith = dailyBonus.RewardFaith;
        }
    }

    public Mission ToMission()
    {
        switch (missionType)
        {
            case nameof(DailyBonusMission):
                DailyBonusMission dailyBonusMission = new DailyBonusMission(id, name, goal, rewardFaith, DateTime.ParseExact(createdTime, CONSTANTSDATE.FORMAT, null), DateTime.ParseExact(untilTime, CONSTANTSDATE.FORMAT, null)){ };
                dailyBonusMission.SetProgress(progress);
                dailyBonusMission.SetIsCompleted(isCompleted);
                dailyBonusMission.SetIsGetReward(isGetReward);
                return dailyBonusMission;
            default:
                throw new ArgumentException($"Unknown mission type: {missionType}");
        }
    }
}

// Mission リストをシリアライズするためのクラス
[Serializable]
public class MissionsList
{
    public List<SerializableMission> missions = new List<SerializableMission>();

    public MissionsList() { } // パラメータなしのコンストラクタが必要です

    public MissionsList(List<Mission> missionList)
    {
        foreach (var mission in missionList)
        {
            missions.Add(new SerializableMission(mission));
        }
    }

    public List<Mission> GetMissions()
    {
        List<Mission> result = new List<Mission>();
        foreach (var serializableMission in missions)
        {
            result.Add(serializableMission.ToMission());
        }
        return result;
    }
}

// Mission リストと JSON 文字列を変換するユーティリティクラス
public static class MissionListJsonConverter
{
    public static string ToJson(List<Mission> missionList)
    {
        MissionsList serializableList = new MissionsList(missionList);
        return JsonUtility.ToJson(serializableList);
    }

    public static List<Mission> FromJson(string json)
    {
        MissionsList serializableList = JsonUtility.FromJson<MissionsList>(json);
        return serializableList.GetMissions();
    }
}