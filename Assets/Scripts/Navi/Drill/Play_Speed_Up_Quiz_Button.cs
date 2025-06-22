using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play_Speed_Up_Quiz_Button : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Speed_Up_Quiz_Scene");
    }
}
