using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home_Button : MonoBehaviour
{
    public void OnClick()
        {
            SceneManager.LoadScene("Home_Scene");
        }
}
