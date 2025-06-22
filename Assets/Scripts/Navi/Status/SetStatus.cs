using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static DataManager;

public class SetStatus : MonoBehaviour
{
    public ScrollRect scrollRect;

    DataManager dataManager;
    List<GameObject> gameObjects = new();
    List<Image> image = new(),image2 = new();
    List<Day_Play_Data> datas = new();
    MasterData master;
    GameObject playBar;
    string format = "yyyy-MM-dd_HH:mm:ss.fff";

    private void Start()
    {
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();
        playBar = Resources.Load<GameObject>("Prehabs/playBar");
        master = dataManager.LoadMaster();

        LoadGraph(10);
    }

    public void LoadGraph(int dayCount)
    {
        int deltaTime;

        while (gameObjects.Count > 0)
        {
            Destroy(gameObjects[0]);
            gameObjects.RemoveAt(0);
        }
        image.Clear();
        image2.Clear();
        datas.Clear();

        if (dayCount != 0)
        {
            deltaTime = dayCount - 1;
        }
        else
        {
            DateTime FromDate = DateTime.ParseExact(master.create_time, format, null);
            DateTime ToDate = DateTime.Today;
            deltaTime = (int)Math.Ceiling((ToDate - FromDate).TotalDays);

            print(FromDate.ToString(CONSTANTSDATE.DAYFORMAT));
            print(ToDate.ToString(CONSTANTSDATE.DAYFORMAT));
        }
        print(deltaTime);


        for (int i = 0; i < deltaTime + 1; i++)
        {
            GameObject bar = Instantiate(playBar);
            gameObjects.Add(bar);
            GameObject obj = bar.transform.GetChild(0).gameObject;
            GameObject obj2 = bar.transform.GetChild(1).gameObject;
            bar.transform.SetParent(GameObject.Find("Content").transform, false);
            image.Add(obj.GetComponent<Image>());
            image2.Add(obj2.GetComponent<Image>());
        }

        int max = 0, max2 = 0;

        for (int i = 0; i < image.Count; i++)
        {
            Day_Play_Data data = dataManager.load_day_play_data(DateTime.Today.AddDays(-i).ToString(CONSTANTSDATE.DAYFORMAT), isMakeFile: false);
            datas.Insert(0, data);
            if (max < data.get_quiz_count())
                max = data.get_quiz_count();
            if (max2 < data.get_correct_count())
                max2 = data.get_correct_count();
        }

        for (int i = 0; i < datas.Count; i++)
        {
            image[i].fillAmount = (float)datas[i].get_quiz_count() / max;
            image2[i].fillAmount = (float)datas[i].get_correct_count() / max;

            image[i].transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().text = datas[i].get_quiz_count().ToString();
            image[i].transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>().text = DateTime.ParseExact(datas[i].date, "yyyy-MM-dd", null).ToString("MM/dd");
        }

        StartCoroutine(MoveToRight());
    }

    IEnumerator MoveToRight()
    {
        yield return null;
        Center();
        scrollRect.horizontalNormalizedPosition = 1f;
        
    }

    void Center()
    {
        // ビューポートの幅
        float viewportWidth = scrollRect.viewport.rect.width;

        // コンテンツの幅
        float contentWidth = scrollRect.content.rect.width;

        if (contentWidth < viewportWidth)
        {
            // コンテンツの幅がビューポートの幅より小さい場合、中央揃えにする
            float offset = (viewportWidth - contentWidth) / 2;
            print(offset + " * 2 = " + viewportWidth + " - " + contentWidth);
            scrollRect.viewport.anchoredPosition = new Vector2(offset, scrollRect.viewport.anchoredPosition.y);
        }
        else
        {
            // コンテンツの幅がビューポートの幅より大きい場合、左揃えに戻す
            scrollRect.viewport.anchoredPosition = new Vector2(0, scrollRect.viewport.anchoredPosition.y);
        }
    }
}
