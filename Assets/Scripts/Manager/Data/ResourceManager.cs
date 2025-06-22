using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using static DataManager;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    DataManager dataManager;
    Basic_Data data;
    string[] resourcesName = CONSTANTSRESOURCE.NAME; // 現在の資源
    Dictionary<string, int> firstResources = new(); // 初期の資源の値


    private void Awake()
    {
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>(); // 相互参照
        data = dataManager.load_basic_data("Resources");
        foreach (string str in resourcesName)
        {
            if (!data.Exist(str)) // 存在しないなら追加
            {
                data.add(new Every_Basic_Data(str, 0));
                print(str + " is added");
            }
            dataManager.SaveBasicData(data);
            firstResources.Add(str, Get(str));
        }
    }

    int Get(string name)
    {
        bool isExist = data.Exist(name);
        if (isExist)
        {
            return data.get(name);
        }
        else
        {
            throw new Exception("This given name is not exist. (Get)");
        }
    }

    public int Get(GameResource resource)
    {
        bool isExist = data.Exist(resourcesName[(int)resource]);
        if (isExist)
        {
            return data.get(resourcesName[(int)resource]);
        }
        else
        {
            throw new Exception("This given name is not exist. (Get)");
        }
    }

    void Set(string name, int value)
    {
        bool isExist = data.Exist(name);
        if (isExist)
        {
            data.set(name, value);
        }
        else
        {
            throw new Exception("This given name is not exist. (Set)");
        }
        dataManager.SaveBasicData(data);
    }

    public Dictionary<string, int> GetFirstResources()
    {
        return firstResources;
    }

    public Dictionary<string, int> GetNowResources()
    {
        Dictionary<string, int> nowResources = new();
        foreach(string str in resourcesName)
        {
            nowResources.Add(str, Get(str));
        }
        return nowResources;
    }

    void Add(string name, int delta)
    {
        bool isExist = data.Exist(name);
        if (isExist)
        {
            int value = data.get(name);
            data.set(name, value + delta);
        }
        else
        {
            throw new Exception("This given name is not exist. (Set)");
        }
        dataManager.SaveBasicData(data);
    }

    public void Add(GameResource resource, int delta)
    {
        bool isExist = data.Exist(resourcesName[(int)resource]);
        if (isExist)
        {
            int value = data.get(resourcesName[(int)resource]);
            data.set(resourcesName[(int)resource], value + delta);
        }
        else
        {
            throw new Exception("This given name is not exist. (Set)");
        }
        dataManager.SaveBasicData(data);
    }

    public void SetText(string name, TextMeshProUGUI text)
    {
        bool isExist = data.Exist(name);
        if (isExist)
        {
            text.text = Get(name).ToString();
        }
        else
        {
            throw new Exception("This given name is not exist. (SetText)");
        }
    }
}
