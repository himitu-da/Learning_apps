using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play_Button : MonoBehaviour
{
    public void OnClick()
        {
            SceneManager.LoadScene("4_choice_quiz");
        }
}
