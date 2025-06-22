using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.UI;
using TMPro;

public class Now_Time : MonoBehaviour
{
    //テキストに関しては、下で一旦GameObject型で宣言してGetComponentする方法と、
    //TextMeshProUGUI型で宣言して変更する方法があったが、後者にする
    DateTime now_time;
    public TextMeshProUGUI Time_Text; // Textオブジェクト

    //private float timeCounter = 0f;
    //private float timeInterval = 0.05f;

    // Update is called once per frame
    void Update()
    {
        now_time = DateTime.Now;
        Time_Text.text = now_time.ToString();
    }
}