using Lean.Gui;
using Lean.Transition;
using Lean.Transition.Method;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetDatabase : MonoBehaviour
{
    public GameObject[] gameObjects, buttonObjects;
    private string[] texts = CONSTANTS.WORDSETNAME;
    public TextMeshProUGUI text, cantPlayText, currentWordSetText;
    DatabaseManager databaseManager;
    int num;

    private void Start()
    {
        int type = PlayerPrefs.GetInt("Gold", 0);
        text.text = "現在：" + texts[type];
        currentWordSetText.text = texts[type];
        SetColor();

        databaseManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DatabaseManager>();
        CheckPlayable(type);

        for (int i = 0; i < gameObjects.Length; i++)
        {
            int index = i;
            gameObjects[index].GetComponent<LeanButton>().OnClick.AddListener(() => SetDatabaseID(index));
            gameObjects[index].GetComponentInChildren<TextMeshProUGUI>().text =
                $"{texts[index]} ({databaseManager.UnlockDao.GetUnlockIDCount(index)}/{databaseManager.WordDao.GetAllIDCount(index)})";
        }
    }

    private void SetDatabaseID(int index)
    {
        PlayerPrefs.SetInt("Gold", index);
        text.text = "現在：" + texts[index];
        currentWordSetText.text = texts[index];
        //SetColor();
        CheckPlayable(index);
    }

    private void CheckPlayable(int index)
    {
        if (databaseManager.UnlockDao.GetUnlockIDCount(index) < 10)
        {
            foreach (GameObject g in buttonObjects)
            {
                g.GetComponent<LeanButton>().interactable = false;
                g.GetComponentsInChildren<Image>()[1].color = Color.gray;
            }
            cantPlayText.enabled = true;
        }
        else
        {
            foreach (GameObject g in buttonObjects)
            {
                g.GetComponent<LeanButton>().interactable = true;
                g.GetComponentsInChildren<Image>()[1].color = new Color(8f / 255f, 148f / 255f, 247f / 255f);
            }
            cantPlayText.enabled = false;
        }
    }

    public Camera cameras;
    private void SetColor()
    {
        num = PlayerPrefs.GetInt("Gold", 0);
    }


        /*
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

    int beforeNum = 0;
    private void Update()
    {
        if (beforeNum != num)
        {
            ChangeButtonsColor();
        }
        num = beforeNum;
    }

    private void ChangeButtonsColor()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Image img = gameObjects[i].transform.GetChild(1).GetComponent<Image>();
            if (i == num)
            {
                img.color = Color.blue;
            }
            else
            {
                img.color = CONSTANTS.BUTTONCOLOR;
            }
        }
    }*/
}
