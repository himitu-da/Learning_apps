using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Lean.Gui;

public class Highlight : MonoBehaviour
{
    public Image[] images;
    public LeanButton[] buttons;
    void Start()
    {
        for (int i = 0; i < CONSTANTS.NAVISCENELIST.Length; i++)
        {
            if(SceneManager.GetActiveScene().name == CONSTANTS.NAVISCENELIST[i])
            {
                print(CONSTANTS.NAVISCENELIST[i]);
                images[i].color = new Color(9f/255, 118f/255, 195f/255);
                buttons[i].interactable = false;
            }
        }
    }
}
