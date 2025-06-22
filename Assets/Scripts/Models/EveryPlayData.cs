using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Every_Play_Data
{
    public Guid uuid = Guid.NewGuid();
    public string date = DateTime.Now.ToString(CONSTANTSDATE.FORMAT);
    public string type;
    public string name;
    public int quiz_count = 0;
    public int correct_count = 0;

}
