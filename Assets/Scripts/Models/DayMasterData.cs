using System;
using System.Collections.Generic;

[System.Serializable]
public class DayMasterData
{
    public string date;
    public List<DayMasterDataEntry> entries = new List<DayMasterDataEntry>();
}

[System.Serializable]
public class DayMasterDataEntry
{
    public string day;
    public string path;
}
