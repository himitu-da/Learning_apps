using Cysharp.Threading.Tasks.Triggers;
using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginBonus : MonoBehaviour
{
    DataManager dataManager;
    //DatabaseManager databaseManager;
    public GameObject buttonObj;
    public GameObject dailyBonusButtonObj;
    LeanButton dailyBonusButton;
    Image dailyBonusImage;
    //public GameObject countDownObj;
    //TextMeshProUGUI countDownTmp;
    public SetBalls setBalls;

    private void Awake()
    {
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();
        //databaseManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DatabaseManager>();

        //countDownTmp = countDownObj.GetComponent<TextMeshProUGUI>();

    }

    async void Start()
    {
        // 初回ログイン
        int isFirstLogin = PlayerPrefs.GetInt("FL", 0);
        if (isFirstLogin == 0)
        {
            buttonObj.SetActive(true);
            dailyBonusButtonObj.SetActive(false); // ボタンの二重表示の防止
        }
        else
        {
            buttonObj.SetActive(false);
            DailyBonus(); // 初回ログインボーナス獲得後のみ表示
        }
    }

    async void DailyBonus() {
        // デイリーボーナスを満たしているとき
        Dictionary<string, MissionData> dailyMissionDataDictionary = await dataManager.achi.missionRepository.GetValidMissionDataDictionaryByType(CONSTANTSMISSION.TYPE[0]);
        MissionData dailyMissionData = dailyMissionDataDictionary[CONSTANTSMISSION.NAME[0, 0]];

        dailyBonusButton = dailyBonusButtonObj.GetComponent<LeanButton>();
        dailyBonusImage = dailyBonusButtonObj.transform.GetChild(1).GetComponent<Image>();

        if (!dailyMissionData.IsGetReward)
        {
            if (dailyMissionData.IsCompleted)
            {
                dailyBonusButton.interactable = true;
                dailyBonusButton.OnClick.AddListener(() =>
                {
                    dataManager.achi.missionRepository.SetIsGetReward(dailyMissionData, true);
                    GetDailyBonus(50);
                });
            }
            else
            {
                dailyBonusButton.interactable = false;
                dailyBonusImage.color = Color.gray;
            }
            dailyBonusButtonObj.SetActive(true);
        }
        else
        {
            dailyBonusButtonObj.SetActive(false);
        }
    }

    public void OnClick() // ログインボーナス
    {
        PlayerPrefs.SetInt("FL", 1);
        dataManager.res.Add(GameResource.Faith, 1000);
        buttonObj.SetActive(false);
        setBalls.GenBalls(1000, true);
        setBalls.UpdateFaith();
        DailyBonus();
    }

    public void GetDailyBonus(int faith)
    {
        dailyBonusButton.interactable = false;
        dataManager.res.Add(GameResource.Faith, faith);
        dailyBonusButtonObj.SetActive(false);
        setBalls.GenBalls(50, true);
        setBalls.UpdateFaith();
    }
}
