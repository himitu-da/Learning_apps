using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Random_Quiz : MonoBehaviour
{
    public GameObject toggle;
    private LeanToggle toggle_toggle;

    private int randamize = 0;
    private string randamize_key = "Randamize_Quiz_Setting";

    public void Start()
    {
        toggle_toggle = toggle.GetComponent<LeanToggle>();

        randamize = PlayerPrefs.GetInt(randamize_key, 0);

        if (randamize == 0)
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
        randamize = 1;
        PlayerPrefs.SetInt(randamize_key, randamize);
    }
    public void Off()
    {
        randamize = 0;
        PlayerPrefs.SetInt(randamize_key, randamize);
    }
}