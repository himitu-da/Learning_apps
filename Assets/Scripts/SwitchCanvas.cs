using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCanvas : MonoBehaviour
{
    public Canvas canvas;

    private void Start()
    {
        canvas.enabled = false;
    }

    public void OnClick() {
        print("Clicked!");
        if (canvas.enabled)
        {
            canvas.enabled = false;
        }
        else
        {
            canvas.enabled = true;
        }
        }
}