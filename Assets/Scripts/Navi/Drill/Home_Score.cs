using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static DataManager;

public class Home_Score : MonoBehaviour
{
    public GameObject english_word_obj;
    public GameObject endress_obj;
    public GameObject speed_up_obj;
    public GameObject sum_obj;
    private TextMeshProUGUI english_word_tmp;
    private TextMeshProUGUI endress_tmp;
    private TextMeshProUGUI speed_up_tmp;
    private TextMeshProUGUI sum_tmp;

    DataManager data_manager;
    Day_Play_Data day_play_data;
    Basic_Data basic_data;

    // Start is called before the first frame update
    
    void Start()
    {
        data_manager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();
        day_play_data = data_manager.load_today_play_data();

        english_word_tmp = english_word_obj.GetComponent<TextMeshProUGUI>();
        endress_tmp = endress_obj.GetComponent<TextMeshProUGUI>();
        speed_up_tmp = speed_up_obj.GetComponent<TextMeshProUGUI>();
        sum_tmp = sum_obj.GetComponent<TextMeshProUGUI>();

        Debug.Log("Start loading basic_data");

        basic_data = data_manager.load_basic_data("English_Word_Quiz");
        int english_word_ever_score = basic_data.get("Ever_Correct_Count");
        int english_word_today_score = day_play_data.get_correct_count("English_Word_Quiz");

        basic_data = data_manager.load_basic_data("Speed_Up_Quiz");
        int speed_up_ever_score = basic_data.get("Ever_Correct_Count");
        int speed_up_today_score = day_play_data.get_correct_count("Speed_Up_Quiz");

        basic_data = data_manager.load_basic_data("Endress_Quiz");
        int endress_ever_score = basic_data.get("Ever_Correct_Count");
        int endress_today_score = day_play_data.get_correct_count("Endress_Quiz");

        Debug.Log("Finished loading basic_data");

        /*
        int english_word_ever_score = PlayerPrefs.GetInt("English_Word_Quiz_Ever_Score", 0); 
        int english_word_today_score = PlayerPrefs.GetInt(get_now_date("English_Word_Quiz_Day_Score"), 0);

        int speed_up_ever_score = PlayerPrefs.GetInt("Speed_Up_Quiz_Ever_Score", 0);
        int speed_up_today_score = PlayerPrefs.GetInt(get_now_date("Speed_Up_Quiz_Day_Score"), 0);

        int endress_ever_score = PlayerPrefs.GetInt("Endress_Question_Ever_Score", (PlayerPrefs.GetInt("Ever_Score_Endress_Question", 0)));
        int endress_today_score = PlayerPrefs.GetInt(get_now_date("Endress_Question_Day_Score"), 0);
        */

        int sum_ever_score = english_word_ever_score + speed_up_ever_score + endress_ever_score;
        int sum_today_score = english_word_today_score + speed_up_today_score+ endress_today_score;

        set_score_text(english_word_tmp, english_word_today_score, english_word_ever_score);
        set_score_text(endress_tmp, endress_today_score, endress_ever_score);
        set_score_text(speed_up_tmp, speed_up_today_score, speed_up_ever_score);
        set_score_text(sum_tmp, sum_today_score, sum_ever_score);
    }

    void set_score_text(TextMeshProUGUI text, int today_score, int ever_score)
    {
        text.text = "<size=24>ç°ì˙ </size>" + today_score + "<size=24> ñ‚ê≥â</size>\n<size=24>ó›êœ </size>" + ever_score + "<size=24> ñ‚ê≥â</size>";
    }

    string get_now_date(string text)
    {
        return text + "_" + DateTime.Now.ToString("yyyy_MM_dd");
    }

    /*
    private void Update()
    {
        DateTime now = DateTime.Now;
        TextMeshProUGUI text = texttest.GetComponent<TextMeshProUGUI>();
        if (now.Second % 2 == 0)
        {
            text.text = Application.persistentDataPath + " " + Application.temporaryCachePath;
        }
        else
        {
            text.text = "Test";
        }
    }*/
}