using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Lean.Gui;

public class SortKind : MonoBehaviour
{
    public GameObject[] objects;
    public string[] objectNames;
    public TextMeshProUGUI text;

    private void Start()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            int index = i;
            objects[i].GetComponent<LeanButton>().OnClick.AddListener(() => OnClick(index));
        }

        int nowType = PlayerPrefs.GetInt("Sort_Type", 0);
        OnClick(nowType);
    }
    void OnClick(int index)
    {
        PlayerPrefs.SetInt("Sort_Type", index);
        text.text = "åªç›ÅF" + objectNames[index];
    }
}
