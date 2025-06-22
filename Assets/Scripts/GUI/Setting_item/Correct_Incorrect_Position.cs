using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class Correct_Incorrect_Position : MonoBehaviour
{
    public GameObject[] correct_incorrect;
    private RectTransform[] correct_incorrect_rect;
    private TextMeshProUGUI[] correct_incorrect_tmp;
    private string RorL_key = "RorL_Correct_Incorrect_Position";

    private int check_change;
    private int check_change_before;

    void Start()
    {
        check_change = PlayerPrefs.GetInt(RorL_key);
        correct_incorrect_rect = new RectTransform[correct_incorrect.Length];
        correct_incorrect_tmp = new TextMeshProUGUI[correct_incorrect.Length];

        for (int i = 0; i < correct_incorrect.Length; i++)
        {
            correct_incorrect_rect[i] = correct_incorrect[i].GetComponent<RectTransform>();
            correct_incorrect_tmp[i] = correct_incorrect[i].GetComponent<TextMeshProUGUI>();
        }
        ChangePosition(check_change);
        print(check_change.ToString() + "OK?");
    }

    void Update()
    {
        check_change = PlayerPrefs.GetInt(RorL_key);
        if (check_change != check_change_before)
        {
            ChangePosition(check_change);
        }
        check_change_before = check_change;
    }

    void ChangePosition(int type)
    {
        if(type == 0)
        {
            ChangeRectX(-250);
            ChangeTextColorAlpha(1f);
        } else if(type == 1)
        {
            ChangeRectX(0);
            ChangeTextColorAlpha(0.75f);
        } else if(type == 2)
        {
            ChangeRectX(250);
            ChangeTextColorAlpha(1f);
        }
        print("Chenged Positon");
    }

    void ChangeRectX(int x)
    {
        foreach (RectTransform rect in correct_incorrect_rect)
        {
            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
        }
    }

    void ChangeTextColorAlpha(float alpha)
    {
        foreach (TextMeshProUGUI tmp in correct_incorrect_tmp)
        {
            Color c = tmp.color;
            c.a = alpha;
            tmp.color = c;
        }
    }
}
