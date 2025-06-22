using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElapsedTime : MonoBehaviour
{
    private float time;
    private bool isRunning;
    private int index;

    public ElapsedTime()
    {
        time = 0f;
        index = 0;
        isRunning = false;
    }

    public void TimerReset()
    {
        time = 0f;
    }

    async public void TimerStart(TextMeshProUGUI timeTmp)
    {
        isRunning = true;
        while (isRunning)
        {
            time += Time.deltaTime;
            timeTmp.text = this.time.ToString("F1") + "•b";
            await UniTask.Yield();
        }
    }

    public void TimerStop()
    {
        isRunning = false;
    }

    public float GetTime()
    {
        return time;
    }
}
