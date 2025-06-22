using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Lean.Gui;

public class MarkPosition : MonoBehaviour
{
    public GameObject[] objects;
    public string[] objectNames;
    public TextMeshProUGUI text;
    private string key = "RorL_Correct_Incorrect_Position";

    private void Start()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            int index = i;
            objects[i].GetComponent<LeanButton>().OnClick.AddListener(() => OnClick(index));
        }

        int nowType = PlayerPrefs.GetInt(key, 0);
        OnClick(nowType);
    }
    void OnClick(int index)
    {
        PlayerPrefs.SetInt(key, index);
        text.text = "åªç›ÅF" + objectNames[index];
    }
}
