using Cysharp.Threading.Tasks;
using Lean.Gui;
using SQLiteUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SetWord : MonoBehaviour
{
    public GameObject[] words;
    public LeanButton[] buttons;
    public TextMeshProUGUI titleTextTmp;
    public LeanButton goToDrillButton;
    int currentIndex = 0, indexCount = 0;
    SQLiteTable<SQLiteRow> wordSet;
    int currentWordSetIndex;
    List<int> unlockWordSetIDList;
    DatabaseManager databaseManager;
    bool isPressedRight = false, isPressedLeft = false;

    private void Start()
    {
        databaseManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DatabaseManager>();
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].OnClick.AddListener(() => SetTextAssetByIndex(index));
            buttons[index].GetComponentInChildren<TextMeshProUGUI>().text =
                $"{CONSTANTS.WORDSETNAME[index]} ({databaseManager.UnlockDao.GetUnlockIDCount(index)}/{databaseManager.WordDao.GetAllIDCount(index)})";
        }
    }

    public void SetTextAssetByIndex(int index, int setIndex = 0, int setText = 5)
    {
        /*
        TextAsset textAsset = databaseManager.GetWordSetByIndex(index);
        if (index == 0)
        wordSet = databaseManager.ParseToWordSet(textAsset, 2, 6);
        else
            wordSet = databaseManager.ParseToWordSet(textAsset, 1, 0);
        */
        wordSet = databaseManager.WordDao.GetSQLiteWordSet(index);
        currentWordSetIndex = index;
        unlockWordSetIDList = databaseManager.UnlockDao.GetUnlockIDList(currentWordSetIndex);
        indexCount = wordSet.Rows.Count;
        SetIndex(setIndex);
        SetText(setText);
        titleTextTmp.text = $"{CONSTANTS.WORDSETNAME[index]} ({databaseManager.UnlockDao.GetUnlockIDCount(index)}/{databaseManager.WordDao.GetAllIDCount(index)})";
        goToDrillButton.OnClick.RemoveAllListeners();
        goToDrillButton.OnClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("Gold", index);
            SceneManager.LoadScene(CONSTANTS.NAVISCENELIST[2]);
        });
    }

    public int GetCurrectWordSetIndex()
    {
        return currentWordSetIndex;
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    public void SetText(int count)
    {
        int delta = 0;
        if (indexCount < currentIndex + count)
        {
            delta = -(currentIndex % count);
        }
        for (int i = 0; i < count; i++)
        {
            Transform t = words[i].transform;
            TextMeshProUGUI from = t.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI to = t.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI num = t.GetChild(4).GetComponent<TextMeshProUGUI>();
            int matchIndex = currentIndex + delta + i;
            SQLiteRow word;

            if (matchIndex >= indexCount)
            {
                from.text = "";
                to.text = "";
                num.text = "";
            }
            else if (matchIndex < indexCount && unlockWordSetIDList.Contains((int)(word = wordSet.Rows[matchIndex])["ID"]))
            {
                from.text = word["Vocab"].ToString();
                to.text = word["Exp"].ToString();
                num.text = (matchIndex + 1).ToString();
            }
            else if (matchIndex < indexCount)
            {
                from.text = "LOCKED";
                to.text = "LOCKED";
                num.text = (matchIndex + 1).ToString();
            }
        }
    }

    /*public void SetText(int index, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform t = words[i].transform;
            t.GetChild(4).GetComponent<TextMeshProUGUI>().text = (currentIndex + i + 1).ToString();
            TextMeshProUGUI from = t.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI to = t.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (wordSet[currentIndex + i][0] != null)
                from.text = wordSet[index + i][0];
            else
                from.text = "";
            if (wordSet[currentIndex + i][1] != null)
                to.text = wordSet[index + i][1];
            else
                to.text = "";
        }
    }
    */

    public void SetIndex(int index)
    {
        if (index >= 0)
        {
            currentIndex = index;
        }
        else
        {
            currentIndex = indexCount - index;
        }
        CheckIndex();
    }

    public void DeltaIndex(int delta)
    {
        currentIndex += delta;
        CheckIndex();
    }

    void CheckIndex()
    {
        if (currentIndex >= indexCount)
            currentIndex = indexCount - 1;
        if (currentIndex < 0)
            currentIndex = 0;
    }

    public void OnRightPointerDown()
    {
        isPressedRight = true;
        ChangeListByTimeRight(Time.time);
    }

    public void OnLeftPointerDown()
    {
        isPressedLeft = true;
        ChangeListByTimeLeft(Time.time);
    }

    public void OnRightPointerUp()
    {
        isPressedRight = false;
    }

    public void OnLeftPointerUp()
    {
        isPressedLeft = false;
    }

    async void ChangeListByTimeRight(float pressTime)
    {
        float pressing_time, longPressDuration = 0.5f;
        while (isPressedRight)
        {
            pressing_time = Time.time - pressTime;
            if (pressing_time > longPressDuration)
            {
                DeltaIndex(5);
                SetText(5);
                await UniTask.Delay(TimeSpan.FromMilliseconds(Math.Max(100 - 20 * pressing_time, 0)));
            }
            await UniTask.Delay(10);
        }
    }

    async void ChangeListByTimeLeft(float pressTime)
    {
        float pressing_time, longPressDuration = 0.5f;
        while (isPressedLeft)
        {
            pressing_time = Time.time - pressTime;
            if (pressing_time > longPressDuration)
            {
                DeltaIndex(-5);
                SetText(5);
                await UniTask.Delay(TimeSpan.FromMilliseconds(Math.Max(100 - 20 * pressing_time, 0)));
            }
            await UniTask.Delay(10);
        }
    }
}
