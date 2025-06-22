using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GaugeValueChange : MonoBehaviour
{
    public GameObject levelUpPointObj, levelDownPointObj, correctPointObj, incorrectPointObj;
    private TMP_InputField levelUpPointInput, levelDownPointInput, correctPointInput, incorrectPointInput;

    public GameObject gaugeIncorrectObj, gaugeStartObj, gaugeCorrectObj;
    private Image gaugeIncorrectImage, gaugeStartImage, gaugeCorrectImage;

    public GameObject levelUpPointTObj, levelDownPointTObj, startPointTObj;
    private TextMeshProUGUI levelUpPointTmp, levelDownPointTmp;
    private RectTransform startPointRect;

    public GameObject percentageObj;
    private TextMeshProUGUI percentageTmp;

    private int levelUpPoint = 100, levelDownPoint = -100, correctPoint = 25, incorrectPoint = -25;
    private readonly string levelUpPointKey = "Level_Up_Point", levelDownPointKey = "Level_Down_Point", correctPointKey = "Correct_Point", incorrectPointKey = "Incorrect_Point";

    public event Action<int, int, int, int> OnGaugeChange;

    private void Awake()
    {
        levelUpPoint = PlayerPrefs.GetInt(levelUpPointKey, 100);
        levelDownPoint = PlayerPrefs.GetInt(levelDownPointKey, -100);
        correctPoint = PlayerPrefs.GetInt(correctPointKey, 25);
        incorrectPoint = PlayerPrefs.GetInt(incorrectPointKey, -25);
    }

    void Start()
    {
        levelUpPointInput = levelUpPointObj.GetComponent<TMP_InputField>();
        levelDownPointInput = levelDownPointObj.GetComponent<TMP_InputField>();
        correctPointInput = correctPointObj.GetComponent<TMP_InputField>();
        incorrectPointInput = incorrectPointObj.GetComponent<TMP_InputField>();

        gaugeIncorrectImage = gaugeIncorrectObj.GetComponent<Image>();
        gaugeStartImage = gaugeStartObj.GetComponent<Image>();
        gaugeCorrectImage = gaugeCorrectObj.GetComponent<Image>();

        levelUpPointTmp = levelUpPointTObj.GetComponent<TextMeshProUGUI>();
        levelDownPointTmp = levelDownPointTObj.GetComponent<TextMeshProUGUI>();
        startPointRect = startPointTObj.GetComponent<RectTransform>();

        percentageTmp = percentageObj.GetComponent<TextMeshProUGUI>();

        levelUpPointInput.text = levelUpPoint.ToString();
        levelDownPointInput.text = levelDownPoint.ToString();
        correctPointInput.text = correctPoint.ToString();
        incorrectPointInput.text = incorrectPoint.ToString();

        levelUpPointTmp.text = levelUpPoint.ToString();
        levelDownPointTmp.text = levelDownPoint.ToString();

        ChangeGauge(levelUpPoint, levelDownPoint, correctPoint, incorrectPoint);
    }

    public void ChangeLevelUpPoint()
    {
        if (levelUpPointInput == null)
        {
            Debug.LogError("levelUpPointInput is not assigned.");
            return;
        }

        TMP_InputField fld = levelUpPointInput;

        print("Detect Changeing");
        if(int.TryParse(fld.text, out int result))
        {
            if (result <= 0)
            {
                result = Math.Max(1, Math.Abs(result));
                fld.text = result.ToString();
            }
            
            if(result > 1000)
            {
                result = 1000;
                fld.text = result.ToString();
            }
        } else
        {
            result = 1;
            fld.text = result.ToString();
        }

        levelUpPoint = result;
        levelUpPointTmp.text = levelUpPoint.ToString();
        PlayerPrefs.SetInt(levelUpPointKey, levelUpPoint);

        ChangeGauge(levelUpPoint, levelDownPoint, correctPoint, incorrectPoint);
    }

    public void ChangeLevelDownPoint()
    {
        if (levelDownPointInput == null)
        {
            Debug.LogError("LevelDownPointInput is not assigned.");
            return;
        }

        TMP_InputField fld = levelDownPointInput;

        print("Detect Changeing");
        if (int.TryParse(fld.text, out int result))
        {
            if (result >= 0)
            {
                result = Math.Min(-1, -Math.Abs(result));
                fld.text = result.ToString();
            }
            
            if (result < -1000)
            {
                result = -1000;
                fld.text = result.ToString();
            }
        }
        else
        {
            result = -1;
            fld.text = result.ToString();
        }

        levelDownPoint = result;
        levelDownPointTmp.text = levelDownPoint.ToString();
        PlayerPrefs.SetInt(levelDownPointKey, levelDownPoint);

        ChangeGauge(levelUpPoint, levelDownPoint, correctPoint, incorrectPoint);

    }

    public void ChangeCorrectPoint()
    {
        if (correctPointInput == null)
        {
            Debug.LogError("CorrectPointInput is not assigned.");
            return;
        }

        TMP_InputField fld = correctPointInput;

        print("Detect Changeing");
        if (int.TryParse(fld.text, out int result))
        {
            if (result < 0)
            {
                result = Math.Max(1, Math.Abs(result));
                fld.text = result.ToString();
            }

            if (result > 1000)
            {
                result = 1000;
                fld.text = result.ToString();
            }
        }
        else
        {
            result = 1;
            fld.text = result.ToString();
        }

        correctPoint = result;
        PlayerPrefs.SetInt(correctPointKey, correctPoint);

        ChangeGauge(levelUpPoint, levelDownPoint, correctPoint, incorrectPoint);

    }

    public void ChangeIncorrectPoint()
    {
        if (incorrectPointInput == null)
        {
            Debug.LogError("IncorrectPointInput is not assigned.");
            return;
        }

        TMP_InputField fld = incorrectPointInput;

        print("Detect Changeing");
        if (int.TryParse(fld.text, out int result))
        {
            if (result > 0)
            {
                result = Math.Min(-1, -Math.Abs(result));
                fld.text = result.ToString();
            }

            if (result < -1000)
            {
                result = -1000;
                fld.text = result.ToString();
            }
        }
        else
        {
            result = -1;
            fld.text = result.ToString();
        }

        incorrectPoint = result;
        PlayerPrefs.SetInt(incorrectPointKey, incorrectPoint);

        ChangeGauge(levelUpPoint, levelDownPoint, correctPoint, incorrectPoint);

    }

    public (int, int, int, int) GetPoint()
    {
        return (levelUpPoint, levelDownPoint, correctPoint, incorrectPoint);
    }

    public void ChangeGauge(int lvUp, int lvDw, int cor, int inc)
    {
        //print("inc: " + inc + " cor: " + cor + " lvUp: " + lvUp + " lvDw: " + lvDw);
        float sum = ((float)lvUp - (float)lvDw);
        gaugeIncorrectImage.fillAmount = ((float)inc - (float)lvDw) / sum;
        gaugeStartImage.fillAmount = (0f - (float)lvDw) / sum;
        gaugeCorrectImage.fillAmount = ((float)cor - (float)lvDw) / sum;
        //print("inc: " + incGauge.fillAmount + " srt: " + srtGauge.fillAmount + " cor: " + corGauge.fillAmount);

        startPointRect.anchorMin = new Vector2 (gaugeStartImage.fillAmount, 0f);
        startPointRect.anchorMax = startPointRect.anchorMin;

        percentageTmp.text = (-(float)inc / ((float)cor - (float)inc) * 100).ToString("F1") + "%";

        if (OnGaugeChange != null) 
            OnGaugeChange(lvUp, lvDw, cor, inc);
    }
}
