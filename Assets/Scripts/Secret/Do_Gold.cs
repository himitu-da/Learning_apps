using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Do_Gold : MonoBehaviour
{
    public Camera cameras;
    // Start is called before the first frame update
    private void Start()
    {
        int num = PlayerPrefs.GetInt("Gold", 0);

        switch (num)
        {
            case 0:
                cameras.backgroundColor = new Color(49f / 255f, 77f / 255f, 121f / 255f);
                break;
            case 1:
                cameras.backgroundColor = new Color(80f / 255f, 0, 0);
                break;
            case 2:
                cameras.backgroundColor = new Color(0f, 0f, 0f);
                break;
            case 3:
                cameras.backgroundColor = new Color(64f / 255f, 64f / 255f, 64f / 255f);
                break;
            case 4:
                cameras.backgroundColor = new Color(128f / 255f, 128f / 255f, 128f / 255f);
                break;
            default:
                cameras.backgroundColor = new Color(49f / 255f, 77f / 255f, 121f / 255f);
                break;
        }
    }
    public void OnClick()
    {
        PlayerPrefs.SetInt("Quiz_Count", 0);
        int num = PlayerPrefs.GetInt("Gold", 0);

        switch (num)
        {
            case 0:
                PlayerPrefs.SetInt("Gold", 1);
                cameras.backgroundColor = new Color(80f / 255f, 0, 0);
                break;
            case 1:
                PlayerPrefs.SetInt("Gold", 2);
                cameras.backgroundColor = new Color(0, 0, 0);
                break;
            case 2:
                PlayerPrefs.SetInt("Gold", 3);
                cameras.backgroundColor = new Color(64f / 255f, 64f / 255f, 64f / 255f);
                break;
            case 3:
                PlayerPrefs.SetInt("Gold", 4);
                cameras.backgroundColor = new Color(128f / 255f, 128f / 255f, 128f / 255f);
                break;
            case 4:
                PlayerPrefs.SetInt("Gold", 0);
                cameras.backgroundColor = new Color(49f / 255f, 77f / 255f, 121f / 255f);
                break;
        }
        print(PlayerPrefs.GetInt("Gold"));
    }
}
