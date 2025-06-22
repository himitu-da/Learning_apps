using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Runtime.InteropServices;

public class Level_Decision : MonoBehaviour
{
    //public GameObject[] dots;
    public GameObject gauge;
    public TextMeshProUGUI speed_obj;
    public TextMeshProUGUI level_obj;
    public TextMeshProUGUI max_level_obj, levelUpTmp, levelDownTmp;
    public Boolean status = false;
    public float interval_time;

    public int level;
    public int max_level;
    //private int level_up_score = 6; // このスコアを超えたらレベルアップ
    //private int level_down_score = 0; // このスコアを下回ったらレベルダウン
    //private int dots_score;
    //private List<Image> dots_image = new List<Image>();
    private string interval_time_text;
    private Image gaugeImage;

    //現在のゲージの値、レベルアップに必要な値、レベルダウンに必要な値、正解したときの獲得量、間違えた時の減少量（負の値）
    private int nowPoint, levelUpPoint, levelDownPoint, correctPoint, incorrectPoint;
    private float nowPointFloat;
    public GameObject gvcObj;
    private GaugeValueChange gvc;

    //レベルチェンジ関連
    private Boolean level_change = false;
    public Boolean finish_level_change = true;

    private void Awake()
    {
        gvc = gvcObj.GetComponent<GaugeValueChange>();
        gvc.OnGaugeChange += ChangeBasicPoint;
    }

    void Start()
    {
        //レベルをロード
        level = PlayerPrefs.GetInt("Level", 1);
        max_level = PlayerPrefs.GetInt("Max_Level", 1);
        //dots_score = PlayerPrefs.GetInt("Dots_Score", 3);
        nowPoint = 0;
        (levelUpPoint, levelDownPoint, correctPoint, incorrectPoint) = gvc.GetPoint();

        interval_time = calc_interval_time(level);
        SetLevelText(level, max_level);

        gaugeImage = gauge.GetComponent<Image>();
        ChangeFillAmount(nowPoint);

        ChangeUpDownText();

        /*
        print(dots.Length);
        for (int i = 0; i < dots.Length; i++)
        {
            dots_image.Add(dots[i].GetComponent<Image>());
            dots_image[i].color = Color.gray;
        }
        dots_image[3].color = Color.black;
        */

        status = true;

    }

    void ChangeUpDownText()
    {
        if (incorrectPoint != 0)
            levelDownTmp.text = "DOWN\n" + ((levelDownPoint - nowPoint + 1) / incorrectPoint + 1);
        else
            levelDownTmp.text = "DOWN\nInfinity";

        if (correctPoint != 0)
            levelUpTmp.text = "UP\n" + ((levelUpPoint - nowPoint - 1) / correctPoint + 1);
        else
            levelUpTmp.text = "UP\nInfinity";
    }

    public void ChangeFillAmount(int point) 
    {
        gaugeImage.fillAmount = ((float)point - (float)levelDownPoint) / ((float)levelUpPoint - (float)levelDownPoint);
    }

    async public void ChangePoint(bool isCorrect)
    {
        int deltaPoint;
        if (isCorrect)
        {
            deltaPoint = correctPoint;
        }
        else
        {
            deltaPoint = incorrectPoint;
        }

        if (level <= 1 && nowPoint + deltaPoint < 0)
        {
            nowPoint = levelDownPoint;
        }
        else
        {
            float perDeltaPoint = (float)deltaPoint / 12f / interval_time;
            float lestDeltaPoint = (float)Math.Abs(deltaPoint);
            int finishDeltaPoint = nowPoint + deltaPoint;
            nowPointFloat = (float)nowPoint;

            while (lestDeltaPoint > 0 && nowPoint > levelDownPoint && nowPoint < levelUpPoint)
            {
                nowPointFloat += perDeltaPoint;
                lestDeltaPoint -= Math.Abs(perDeltaPoint);
                nowPoint = (int)Math.Round(nowPointFloat);
                ChangeFillAmount(nowPoint);
                await UniTask.Delay(TimeSpan.FromSeconds(1f / 120f));
            }
            nowPoint = finishDeltaPoint;
            CheckLevelChangeByGauge();
            ChangeUpDownText();
        }

        //レベルを保存
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Max_Level", max_level);

        if (level_change)
        {
            interval_time = calc_interval_time(level);
            SetLevelText(level, max_level);
            level_change = false;
        }

        ChangeFillAmount(nowPoint);
        finish_level_change = true;
    }

    public void ChangeBasicPoint(int lvUp, int lvDw, int cor, int inc)
    {
        levelUpPoint = lvUp;
        levelDownPoint = lvDw;
        correctPoint = cor;
        incorrectPoint = inc;

        ChangeUpDownText();
    }

    public void ChangeLevel(int delta)
    {
        level += delta;
        interval_time = calc_interval_time(level);
        SetLevelText(level, max_level);
    }

    public void CheckLevelChangeByGauge()
    {

        if (nowPoint >= levelUpPoint)
        {
            level += 1;
            nowPoint = 0;
            level_change = true;
        }
        else if (nowPoint <= levelDownPoint)
        {
            level -= 1;
            nowPoint = 0;
            level_change = true;
        }

        if (level > max_level)
        {
            max_level = level;
        }

        if (level < 1)
        {
            level = 1;
            level_change = false;
        }
    }

    /*
    public void change_dots_score(int score)
    {
        if (level <= 1 && nowPoint + deltaPoint < 0)
        {
            nowPoint = 0;
        }
        else
        {
            nowPoint += deltaPoint;
        }

        check_level_change();
        
        //レベルを保存
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Max_Level", max_level);
        PlayerPrefs.SetInt("Dots_Score", dots_score);

        if (level_change)
        {
            interval_time = calc_interval_time(level);
            set_level_text(level, max_level);
            level_change = false;
        }
        finish_level_change = true;
    }

    */
    float calc_interval_time(int num_level)
    {
        if (num_level <= 3)
        {
            return 11f - num_level * 1f;
        }
        else if (num_level <= 5)
        {
            return 9.5f - num_level * 0.5f;
        }
        else if (num_level <= 10)
        {
            return 8f - (num_level * 0.2f);
        }
        else if (num_level <= 30)
        {
            return 7f - (num_level * 0.1f);
        }
        else if (num_level <= 50)
        {
            return 5.5f - (num_level * 0.05f);
        }
        else if (num_level <= 100)
        {
            return 4f - (num_level * 0.02f);
        }
        else if (num_level <= 200)
        {
            return 3f - (num_level * 0.01f);
        }
        else
        {
            return Math.Max(1.2f - num_level * 0.001f, 0.01f);
        }
    }

    /*void Update()
    {
        //set_dots_color();
    }*/

    /*

    void set_dots_color()
    {
        for (int i = 0; i < dots.Length; i++)
        {
            if (i == dots_score)
            {
                dots_image[i].color = Color.black;
            }
            else
            {
                dots_image[i].color = Color.gray;
            }
        }
    }

    */

    /*
    void check_level_change()
    {
        if (dots_score > level_up_score)
        {
            level += 1;
            dots_score = 3;
            level_change = true;

        }
        else if (dots_score < level_down_score)
        {
            level -= 1;
            dots_score = 3;
            level_change = true;
        }

        if (level > max_level)
            max_level = level;
    }
    */

    void SetLevelText(int num_level, int num_max_level)
    {
        max_level_obj.GetComponent<TextMeshProUGUI>().text = "<size=24>BEST </size>" + num_max_level;
        level_obj.GetComponent<TextMeshProUGUI>().text = "<size=24>LEVEL </size>" + num_level;
        if (level < 5) // async有効化したらエラー発生の可能性
        {
            interval_time_text = string.Format("{0:f0}", interval_time) + "s";
        }
        else if (level < 30)
        {
            interval_time_text = string.Format("{0:f1}", interval_time) + "s";
        }
        else if (level < 200)
        {
            interval_time_text = string.Format("{0:f2}", interval_time) + "s";
        }
        else
        {
            interval_time_text = string.Format("{0:f3}", interval_time) + "s";
        }
        speed_obj.GetComponent<TextMeshProUGUI>().text = "<size=24>TIME </size>" + interval_time_text;
    }
}