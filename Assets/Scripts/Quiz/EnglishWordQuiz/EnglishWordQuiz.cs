using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Gui;
using System;
//using UnityEngine.Windows.Speech;
using Cysharp.Threading.Tasks;
using static DataManager;
using UnityEditor;
using Cysharp.Threading.Tasks.Triggers;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using System.Threading;

public class EnglishWordQuiz : MonoBehaviour
{
    //public GameObject question;
    //public GameObject[] buttons_obj;
    public GameObject[] quizCountObj;
    //private List<LeanButton> buttons = new List<LeanButton>();
    //private int correct_index = 0;
    //private TextMeshProUGUI question_tmp;
    //private List<TextMeshProUGUI> choices_tmp = new List<TextMeshProUGUI>();
    //private List<GameObject> correct_marks = new List<GameObject>();
    //private List<GameObject> wrong_marks = new List<GameObject>();
    public GameObject result_canvas;
    public GameObject score_object, elapsedTimeResultObj;
    //public GameObject play_canvas;
    public GameObject Canvas, playSettingCanvas;
    public GameObject statusCanvas;
    public GameObject score_obj;
    private TextMeshProUGUI score_tmp;
    private int max_num_question = 0;
    private float delay_time; //KAMIJO
    //private Boolean do_answer;
    private Boolean end_answer;
    private Boolean isCorrect;
    private Boolean opening_scene = true;

    // 問題数の表示用
    public TextMeshProUGUI question_number_text;

    // オーディオ
    //public AudioClip correct_audio;
    //public AudioClip wrong_audio;
    //private AudioSource audioSource;

    //正解数と出題数
    private int score = 0;
    private int quiz_count = 0;
    private int ever_score; //累積正解数
    private int ever_quiz_count;
    private int today_score;
    private int today_quiz_count;

    //ゲージ
    public Image gaugeImage; // ゲージの画像

    //次の問題の形式
    public Canvas next_canvas;

    DataManager dataManager;
    Every_Play_Data every_play_data;
    Basic_Data basic_data;

    QuizManager quizManager;
    Quiz quiz;

    public GameObject statusObj;
    Scrollview_Object statusTmp;

    public GameObject elaspedTimeObj;
    TextMeshProUGUI elapsedTimeTmp;
    ElapsedTime elapsedTime;

    CancellationTokenSource cts;

    // リソースの獲得量
    public TextMeshProUGUI faithTmp;
    int numFaith = 0;

    private void Awake()
    {
        every_play_data = new Every_Play_Data();
        every_play_data.type = "English_Word_Quiz";
    }

    void Start()
    {
        quizManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<QuizManager>();
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();

        quizManager.ui.GenerateButtonsUI(4);

        LoadComponent();
        LoadData();
        //SetButtons();

        quizManager.OnQuizServed += UpdateWhenQuizServed;
        quizManager.OnCorrect += WhenQuizCorrect;
        quizManager.OnWrong += WhenQuizWrong;

        //sceneUnloadedに関数を追加
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        PlaySetting();
    }

    public void AgainEnglishWordQuiz()
    {
        result_canvas.SetActive(false);
        Canvas.SetActive(true); // 追加
        quizManager.ui.SetActive(false);
        //play_canvas.SetActive(false);
        playSettingCanvas.SetActive(true);
        statusCanvas.SetActive(false);
    }

    void LoadComponent()
    {
        statusTmp = statusObj.GetComponent<Scrollview_Object>();
        elapsedTimeTmp = elaspedTimeObj.GetComponent<TextMeshProUGUI>();

        //question_tmp = question.GetComponent<TextMeshProUGUI>();
        score_tmp = score_obj.GetComponent<TextMeshProUGUI>();
        
        //audioSource = GetComponent<AudioSource>();
        elapsedTime = new();

        SetResourceText(numFaith);
    }

    void LoadData()
    {
        basic_data = dataManager.load_basic_data("English_Word_Quiz");
        ever_score = dataManager.load_basic_data("English_Word_Quiz").get("Ever_Correct_Count");
        ever_quiz_count = dataManager.load_basic_data("English_Word_Quiz").get("Ever_Quiz_Count");

        /*
        today_score = data_manager.load_today_play_data().get_correct_count();
        today_quiz_count = data_manager.load_today_play_data().get_quiz_count();
        */
    }

    /*
    void SetButtons()
    {
        for (int i = 0; i < buttons_obj.Length; i++)
        {
            choices_tmp.Add(buttons_obj[i].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>());
            correct_marks.Add(buttons_obj[i].transform.GetChild(5).gameObject);
            wrong_marks.Add(buttons_obj[i].transform.GetChild(6).gameObject);

            var press_index = i;
            buttons.Add(buttons_obj[i].GetComponent<LeanButton>());
            buttons[i].OnClick.AddListener(() => CheckAnswer(press_index));
        }
    }*/

    /*
    void InteractableButtons(Boolean isInteractive)
    {
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].interactable = isInteractive;
    }

    void EnabledButtons(Boolean isEnabled)
    {
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].enabled = isEnabled;
    }

    void ClearMarks()
    {
        //正解・不正解アイコンを非表示
        correct_marks[correct_index].SetActive(false);
        for (int i = 0; i < buttons.Count; i++)
        {
            wrong_marks[i].SetActive(false);
        }
    }*/

    void PlaySetting()
    {
        quizManager.ui.SetActive(false);
        playSettingCanvas.SetActive(true);
        statusCanvas.SetActive(false);

        int[] ints = new int[] {10, 30, 50, 100, quizManager.GetDatabaseCount() };
        for(int i = 0; i < quizCountObj.Length; i++)
        {
            var index = i;
            var obj = quizCountObj[index];
            obj.GetComponent<LeanButton>().OnClick.AddListener(() => StartQuiz(ints[index]));
            obj.GetComponentInChildren<TextMeshProUGUI>().text = ints[index] + "問";
        }
    }

    async public void StartQuiz(int allQuizCount)
    {
        quizManager.ui.SetActive(true);
        quizManager.ui.SetEnabled(true);
        playSettingCanvas.SetActive(false);
        statusCanvas.SetActive(true);

        elapsedTime.TimerReset();
        elapsedTime.TimerStart(elapsedTimeTmp);
        max_num_question = allQuizCount;
        quiz_count = 0;
        score = 0;
        gaugeImage.fillAmount = 0f;

        quizManager.SetOptionsCount(1, 3);
        SetScoreText(score, quiz_count);

        while (quiz_count < max_num_question && opening_scene)
        {
            //do_answer = false;
            end_answer = false;
            //print(correct_index);
            //print(correct_marks.Count);

            quizManager.ui.SetInteractable(false);
            quizManager.GenMultiQuiz();
            await quizManager.SetQuiz();
            quiz = quizManager.GetQuiz();

            gaugeImage.fillAmount = (float)quiz_count / (float)max_num_question; // ゲージの長さを更新
            //await UniTask.Delay(400);
            //InteractableButtons(true);
            
            cts = new();
            await UniTask.WaitUntil(() => end_answer, cancellationToken: cts.Token);
            statusTmp.AddItem(quiz.GetQuizSentence().ToString(), quiz.GetOptionsList()[quiz.GetCorrectIndexList()[0]].ToString(), isCorrect);
        }

        ShowResult(score, max_num_question);
    }

    void UpdateWhenQuizServed()
    {
        ever_quiz_count++;
        today_quiz_count++;
        quiz_count++;
        question_number_text.text = (quiz_count + "/" + max_num_question).ToString();

        every_play_data.quiz_count = quiz_count;
        dataManager.SaveEveryPlayData(every_play_data);
        basic_data.set("Ever_Quiz_Count", ever_quiz_count);
        dataManager.SaveBasicData(basic_data);
    }

    void WhenQuizCorrect()
    {
        delay_time = 0.3f; //KAMIJO

        score++;
        today_score++;
        ever_score++;
        numFaith++;

        every_play_data.correct_count = score;
        dataManager.SaveEveryPlayData(every_play_data);
        basic_data.set("Ever_Correct_Count", ever_score);
        dataManager.SaveBasicData(basic_data);

        dataManager.res.Add(GameResource.Faith, 1);
        dataManager.achi.DailyBonusAddProgress(1);

        SetResourceText(numFaith);
        isCorrect = true;
        AfterQuiz(delay_time);
    }

    void WhenQuizWrong()
    {
        delay_time = 1.5f; //KAMIJO
        isCorrect = false;
        AfterQuiz(delay_time);
    }

    async void AfterQuiz(float delayTime)
    {
        SetScoreText(score, quiz_count);

        //次の問題設定がOFFのとき
        if (PlayerPrefs.GetInt("Next_Quiz_Setting") == 0)
        {
            next_canvas.enabled = true;
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
            end_answer = true;
        }
    }

    /*
    void SetQuiz()
    {
        quiz = quizManager.GenMultiQuiz(1, 3);
        question_tmp.text = quiz.GetQuizSentence();
        correct_index = quiz.GetCorrectIndexList()[0];
        List<string> english_list = quiz.GetOptionsList();

        for (int i = 0; i < english_list.Count; i++)
        {
            choices_tmp[i].text = english_list[i];
        }


        ever_quiz_count++;
        today_quiz_count++;
        quiz_count++;
        question_number_text.text = (quiz_count + "/" + max_num_question).ToString();

        every_play_data.quiz_count = quiz_count;
        every_play_data.save();
        basic_data.set("Ever_Quiz_Count", ever_quiz_count);
        basic_data.save();
    }*/

    /*
    async public void CheckAnswer(int press_index)
    {
        if (do_answer == true)
        {
            return;
        }

        do_answer = true;

        InteractableButtons(false);

        if (correct_index == press_index)
        {
            print("OK");
            audioSource.PlayOneShot(correct_audio);
            delay_time = 0.3f; //KAMIJO

            score++;
            today_score++;
            ever_score++;

            every_play_data.correct_count = score;
            every_play_data.save();
            basic_data.set("Ever_Correct_Count", ever_score);
            basic_data.save();
            isCorrect = true;
        }
        else
        {
            print("NG");
            audioSource.PlayOneShot(wrong_audio);
            wrong_marks[press_index].SetActive(true);
            delay_time = 1.5f; //KAMIJO
            isCorrect = false;
        }

        correct_marks[correct_index].SetActive(true);
        //quizManager.IsCorrect(isCorrect);
        SetScoreText(score, quiz_count);

        //次の問題設定がOFFのとき
        if(PlayerPrefs.GetInt("Next_Quiz_Setting") == 0)
        {
            next_canvas.enabled = true;
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay_time));
            end_answer = true;
        }
    }*/

    public void OnClick()
    {
        next_canvas.enabled = false;
        end_answer = true;
    }

    /*
    async public void NextQuestion()
    {
        do_answer = false;
        print(correct_index);
        print(correct_marks.Count);

        //正解・不正解アイコンを非表示
        correct_marks[correct_index].SetActive(false);
        for (int i = 0; i < buttons.Count; i++)
        {
            wrong_marks[i].SetActive(false);
        }

        //全問題終了したら結果を表示する
        if (question_index == max_num_question)
        {
            ShowResult(score);
            return;
        }

        (string quiz_sentence, List<string> options, int index) = await dictionary.get_choice_quiz(4);

        // 効果音なし
        question_tmp.text = quiz_sentence;
        correct_index = index;
        List<string> english_list = options;

        for (int i = 0; i < english_list.Count; i++)
        {
            choices_tmp[i].text = english_list[i];
        }
        question_index++;
        question_number_text.text = (question_index + "/" + max_num_question).ToString();

        ever_quiz_count++;
        today_quiz_count++;
        quiz_count++;

        every_play_data.quiz_count = quiz_count;
        every_play_data.save();
        basic_data.set("Ever_Quiz_Count", ever_quiz_count);
        basic_data.save();

        gaugeImage.fillAmount = (float)question_index / (float)max_num_question; // ゲージの長さを更新

        dictionary.shuffle_flag = false;
        dictionary.shuffle();

        await UniTask.Delay(400);

        //ボタン押せるようにする
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].enabled = true;
    }
    */

    private void ShowResult(int score, int maxScore)
    {
        elapsedTime.TimerStop();
        quizManager.ui.SetActive(false);
        result_canvas.SetActive(true);
        Canvas.SetActive(false); // 追加
        statusCanvas.SetActive(false);
        score_object.GetComponent<TextMeshProUGUI>().text = string.Format("{0}問中{1}問正解", maxScore, score);
        elapsedTimeResultObj.GetComponent<TextMeshProUGUI>().text = string.Format("{0}秒", elapsedTime.GetTime().ToString("F1"));
    }
    void SetScoreText(int num_score, int num_quiz_count)
    {
        print(num_score + " " + num_quiz_count);
        score_tmp.text = num_quiz_count + "<size=24> 問中 </size>" + num_score + "<size=24> 問正解</size>";
        //score_tmp.text = num_score + "<size=24> 問正解 </size>/<size=24> 累積 </size>" + num_ever_score + "<size=24> 問正解</size>";
        //ever_score_tmp.text = "<size=24>累積 </size>" + num_ever_score + "<size=24> 問正解 </size>";
    }

    void SetResourceText(int num)
    {
        if (faithTmp != null)
        {
            faithTmp.text = "<sprite name=faith>" + num.ToString();
        }
    }

    void OnSceneUnloaded(Scene current)
    {
        print("OnSceneUnloaded: " + current);
        opening_scene = false;
        try
        {
            cts.Cancel();
        }
        catch { }
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
