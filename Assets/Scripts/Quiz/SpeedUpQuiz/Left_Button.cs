using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class Left_Button : MonoBehaviour
{
    Level_Decision Level_Decision_cs;
    // Start is called before the first frame update
    public void OnClick()
    {
        Level_Decision_cs = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<Level_Decision>();
        if (Level_Decision_cs.level > 1)
            Level_Decision_cs.ChangeLevel(-1);
    }

    private bool isPressed;
    private float pressTime;
    private float longPressDuration = 0.5f; // ’·‰Ÿ‚µ‚ÌŽžŠÔ

    public void OnPointerDown()
    {
        isPressed = true;
        pressTime = Time.time;
        print("Pressing LeftButton");
        change_level();
    }

    public void OnPointerUp()
    {
        isPressed = false;
    }

    async void change_level()
    {
        float pressing_time;
        Level_Decision_cs = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<Level_Decision>();
        while (isPressed)
        {
            pressing_time = Time.time - pressTime;
            if (pressing_time > longPressDuration)
            {
                if (Level_Decision_cs.level > 1)
                    Level_Decision_cs.ChangeLevel(-1);
                await UniTask.Delay(TimeSpan.FromMilliseconds(Math.Max(100 - 20 * pressing_time, 0)));
            }
            await UniTask.Delay(10);
        }

    }
}
