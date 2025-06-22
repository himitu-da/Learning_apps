using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Basic_Data
{
    public string type;
    public List<Every_Basic_Data> every_data = new();

    public void add(Every_Basic_Data data)
    {
        every_data.Add(data);
    }

    public bool Exist(string name)
    {
        bool isExist;
        isExist = every_data.Any(n => n.name == name);
        return isExist;
    }

    public int get(string name)
    {
        int n;
        try
        {
            n = every_data.First(n => n.name == name).status;
        }
        catch
        {
            throw new Exception("This given name is not exist. (get at DataManager)");
        }
        return n;

    }

    public int get(string name, int def)
    {
        int n;
        try
        {
            n = every_data.First(n => n.name == name).status;
        }
        catch
        {
            n = def;
            add(new Every_Basic_Data(name, n));
        }
        return n;

    }

    /// <summary>
    /// セーブはしない。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="status"></param>
    public void set(string name, int status)
    {
        Every_Basic_Data data = every_data.FirstOrDefault(n => n.name == name);
        if (data != null)
        {
            data.status = status;
        }
        else
        {
            data = new Every_Basic_Data();
            data.name = name;
            data.status = status;
            add(data);
        }
    }
}
