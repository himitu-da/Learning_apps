using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;
    public int sceneIndex;
    public void OnClick()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickByIndex()
    {
        print("You are opening to " + CONSTANTS.NAVISCENELIST[sceneIndex]);
        SceneManager.LoadScene(CONSTANTS.NAVISCENELIST[sceneIndex]);
    }
}
