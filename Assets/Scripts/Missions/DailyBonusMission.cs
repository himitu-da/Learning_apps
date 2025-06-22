using System;
using UnityEngine;

[System.Serializable]
public class DailyBonusMission : Mission
{
    public int RewardFaith { get; protected set; }
    public DailyBonusMission(string id, string name, int goal, int rewardFaith, DateTime creatingTime, DateTime untilTime)
        : base(id, CONSTANTSMISSION.TYPE[0], name, "{goal}問正解すると獲得できる", creatingTime, untilTime, goal)
    {
        RewardFaith = rewardFaith;
    }

    protected override void OnMissionCompleted()
    {
        Debug.Log("Daily Bonus Mission Completed");
        // 例えば通知を表示するなど
    }

    public override void UpdateProgress(object value)
    {
        if (value is int count)
            Progress += count;
        CheckCompletion();
    }
}
