using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Next_Quiz : MonoBehaviour
{
    public GameObject toggle;
    private LeanToggle toggle_toggle;

    private int next = 0;
    private string next_key = "Next_Quiz_Setting";

    public void Start()
    {
        toggle_toggle = toggle.GetComponent<LeanToggle>();

        next = PlayerPrefs.GetInt(next_key, 0);

        if (next == 0)
        {
            toggle_toggle.On = false;
        }
        else
        {
            toggle_toggle.On = true;
        }
    }

    public void On()
    {
        next = 1;
        PlayerPrefs.SetInt(next_key, next);
    }
    public void Off()
    {
        next = 0;
        PlayerPrefs.SetInt(next_key, next);
    }
}