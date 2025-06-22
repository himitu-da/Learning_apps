using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Endress_Play_Button : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Endress_Quiz_Scene");
    }
}