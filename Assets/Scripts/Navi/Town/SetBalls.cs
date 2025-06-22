using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class SetBalls : MonoBehaviour
{
    public GameObject ballPrefab; // ボールのプレハブをインスペクタで設定
    List<List<GameObject>> balls = new List<List<GameObject>>();
    float[] size = { 0.2f, 0.45f, 0.7f, 1f, 1.3f, 1.6f };

    DataManager dataManager;
    public TextMeshProUGUI faith, knowledge, sunpower;
    int beforeCount;

    private Boolean opening_scene = true;
    CancellationTokenSource cts = new();

    int count;

    void Start()
    {
        //sceneUnloadedに関数を追加
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        beforeCount = PlayerPrefs.GetInt("BeforeFaith", 0);

        for (int i = 0; i < 6; i++)
            balls.Add(new List<GameObject>());
        count = 0;

        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();
        dataManager.res.SetText("Faith", faith);
        dataManager.res.SetText("Knowledge", knowledge);
        dataManager.res.SetText("SunPower", sunpower);

        Set();
    }

    void Set()
    {
        if (beforeCount < dataManager.res.Get(GameResource.Faith))
        {
            GenBalls(beforeCount, false, -3f, -3.75f, 3f, -3f);
            GenBalls(dataManager.res.Get(GameResource.Faith) - beforeCount, true);
        }
        else
        {
            GenBalls(dataManager.res.Get(GameResource.Faith), false, -3f, -3.75f, 3f, -3f);
        }
        UpdateFaith();
    }

    public void UpdateFaith()
    {
        PlayerPrefs.SetInt("BeforeFaith", dataManager.res.Get(GameResource.Faith));
    }

    public async void GenBalls(int numberOfBalls, bool isInterval, float top = 4f, float btm = 4f, float rig = 0.5f, float lft = -0.5f)
    {
        float sep = 1;
        count = 0;
        while (count < numberOfBalls && opening_scene)
        {
            // ボールのプレハブをインスタンス化
            for (int i = 0; i < (int)sep;)
            {
                if (sep - i >= 100)
                {
                    MakeBall(1, top, btm, rig, lft);
                    count += 100;
                    i += 100;
                    sep += 1f;
                }
                else
                {
                    MakeBall(0, top, btm, rig, lft);
                    count++;
                    i++;
                    sep += 0.01f;
                }
                if (count >= numberOfBalls)
                    break;
            }
            CheckMargeBall(top, btm, rig, lft);

            if (isInterval)
            {
                if (sep <= 2.99f)
                    await UniTask.Delay(TimeSpan.FromSeconds(1f / ((sep - 0.99) * 100)), cancellationToken: cts.Token);
                else
                    await UniTask.Delay(TimeSpan.FromSeconds(1f / 200f), cancellationToken: cts.Token);
            }
        }
    }

    private Vector3 GetRandomPosition(float top, float btm, float rig, float lft)
    {
        // ランダムな位置を生成
        float x = UnityEngine.Random.Range(lft, rig);
        float y = UnityEngine.Random.Range(btm, top);
        return new Vector3(x, y, 0);
    }

    private void MakeBall(int index, float top, float btm, float rig, float lft)
    {

        // ボールのプレハブをインスタンス化
        GameObject newBall = Instantiate(ballPrefab, GetRandomPosition(top, btm, rig, lft), Quaternion.identity);
        // 新しいスケールを設定
        newBall.transform.localScale = new Vector3(size[index], size[index], size[index]);
        balls[index].Add(newBall);
    }

    private void CheckMargeBall(float top = 4f, float btm = 4f, float rig = 0.5f, float lft = -0.5f)
    {
        for (int i = 0; i < 5; i++)
        {
            while (balls[i].Count >= 100)
            {
                MakeBall(i + 1, top, btm, rig, lft);

                for (int j = 0; j < 100; j++)
                {
                    Destroy(balls[i][0]);
                    balls[i].RemoveAt(0);
                }
            }
        }
    }

    void OnSceneUnloaded(Scene current)
    {
        print("OnSceneUnloaded: " + current);
        opening_scene = false;
        cts.Cancel();
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void Update()
    {
        dataManager.res.SetText("Faith", faith);
        dataManager.res.SetText("Knowledge", knowledge);
        dataManager.res.SetText("SunPower", sunpower);
    }
}
