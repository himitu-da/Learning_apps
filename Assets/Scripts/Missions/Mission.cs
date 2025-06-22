using System;
using UnityEngine;

/// <summary>
/// 抽象クラス。IDは1からインクリメントされる。
/// </summary>
[System.Serializable]
public abstract class Mission
{
    public string ID { get; set; }
    /// <summary>
    /// ミッションの大まかなタイプ。DAILYPLAYMISSIONなど
    /// </summary>
    public string Type { get; set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }

    [SerializeField]
    private string _createdTime;
    public DateTime CreatedTime
    {
        get { return DateTime.ParseExact(_createdTime, CONSTANTSDATE.FORMAT, null); }
        protected set { _createdTime = value.ToString(CONSTANTSDATE.FORMAT); }
    }

    [SerializeField]
    private string _untilTime;
    public DateTime UntilTime
    {
        get { return DateTime.ParseExact(_untilTime, CONSTANTSDATE.FORMAT, null); }
        protected set { _untilTime = value.ToString(CONSTANTSDATE.FORMAT); }
    }
    public int Progress { get; protected set; }
    public int Goal { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public bool IsGetReward { get; protected set; }

    public Mission(string id, string type, string name, string description, DateTime createdTime, DateTime untilTime, int goal)
    {
        ID = id;
        Type = type;
        Name = name;
        Description = description;
        CreatedTime = createdTime;
        UntilTime = untilTime;
        Goal = goal;
        Progress = 0;
        IsCompleted = false;
        IsGetReward = false;
    }

    public abstract void UpdateProgress(object progressData);

    public virtual void CheckCompletion()
    {
        if (Progress >= Goal && !IsCompleted)
        {
            IsCompleted = true;
            OnMissionCompleted();
        }
    }

    protected abstract void OnMissionCompleted();

    public virtual string GetProgressText()
    {
        return $"{Progress}/{Goal}";
    }

    public virtual void SetProgress(int progress)
    {
        Progress = progress;
    }

    public virtual void SetIsCompleted(bool isCompleted)
    {
        IsCompleted = isCompleted;
    }

    public virtual void SetIsGetReward(bool isGetReward)
    {
        IsGetReward = isGetReward;
    }
}
