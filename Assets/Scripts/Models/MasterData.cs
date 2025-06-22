using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class MasterData
{
    public string create_time = DateTime.Now.ToString(CONSTANTSDATE.FORMAT);
    public string root_path;
    public List<MasterDataEntry> entries = new List<MasterDataEntry>();


    public void add(MasterDataEntry entry)
    {
        entries.Add(entry);
        if (entry.path.StartsWith("/"))
        {
            entry.path = entry.path.Substring(1);
        }

        Directory.CreateDirectory(Path.Combine(root_path, entry.path));
    }

}

[System.Serializable]
public class MasterDataEntry
{
    public string data;
    public string path;
}
