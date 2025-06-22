using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RorL_Display_Position : MonoBehaviour
{
    public GameObject toggle;
    private LeanToggle toggle_toggle;

    private int RorL = 0;
    private string RorL_key = "RorL_Correct_Incorrect_Position";
    public void Start()
    {
        toggle_toggle = toggle.GetComponent<LeanToggle>();

        RorL = PlayerPrefs.GetInt(RorL_key, 0);

        if(RorL == 0)
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
        RorL = 1;
        PlayerPrefs.SetInt(RorL_key, RorL);
    }
    public void Off()
    {
        RorL = 0;
        PlayerPrefs.SetInt(RorL_key, RorL);
    }


}
