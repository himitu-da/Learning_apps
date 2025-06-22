using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class testtest : MonoBehaviour
{
    public TextMeshProUGUI texttest;

    public void Start()
    {
        // パスの取得
        string directoryPath = Application.persistentDataPath;
        string fileName = "every_play_data.txt";
        string filePath = Path.Combine(directoryPath, fileName);

        // ディレクトリの存在確認と作成
        EnsureDirectoryExists(directoryPath);

        // パスをTextMeshProUGUIに表示
        texttest.text = filePath;
        Debug.Log("Persistent Data Path: " + filePath);

        // ファイルの書き込みと読み込みの例
        WriteToFile(filePath, "Hello, World!");
        string content = ReadFromFile(filePath);
        Debug.Log("File Content: " + content);
    }

    void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("Directory created at: " + path);
        }
    }

    void WriteToFile(string path, string content)
    {
        try
        {
            File.WriteAllText(path, content);
            Debug.Log("File written successfully: " + path);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write file: " + e.Message);
        }
    }

    string ReadFromFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
                return content;
            }
            else
            {
                Debug.LogError("File not found: " + path);
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to read file: " + e.Message);
            return null;
        }
    }
}
