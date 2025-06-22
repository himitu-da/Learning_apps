using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Day_Play_Data
{
    public string date; // yyyy-MM-dd
    public List<Every_Play_Data> every_data = new List<Every_Play_Data>();

    public Day_Play_Data()
    {

    }

    public void add(Every_Play_Data data)
    {
        Boolean exists = every_data.Any(d => d.date == data.date && d.type == data.type);

        if (!exists)
            every_data.Add(data);
    }


    public int get_quiz_count(string type = null)
    {
        int count = 0;
        List<Every_Play_Data> entries = every_data;
        try
        {
            if (type != null)
            {
                entries = every_data.Where(d => d.type == type).ToList();
            }

            foreach (Every_Play_Data data in entries)
            {
                count += data.quiz_count;
            }
        }
        catch { }
        return count;
    }
    public int get_correct_count(string type = null)
    {
        int count = 0;
        List<Every_Play_Data> entries = every_data;
        try
        {
            if (type != null)
            {
                entries = every_data.Where(d => d.type == type).ToList();
            }

            foreach (Every_Play_Data data in entries)
            {
                count += data.correct_count;
            }
        }
        catch { }
        return count;
    }
}
