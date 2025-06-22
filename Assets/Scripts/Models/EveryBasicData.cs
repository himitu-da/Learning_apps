using System;

[System.Serializable]
public class Every_Basic_Data
{
    public string name;
    public int status;

    public Every_Basic_Data()
    {

    }

    public Every_Basic_Data(string name)
    {
        this.name = name;
    }

    public Every_Basic_Data(string name, int status)
    {
        this.name = name;
        this.status = status;
    }
}
