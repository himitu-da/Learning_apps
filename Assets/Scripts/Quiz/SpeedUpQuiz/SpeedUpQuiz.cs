using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using TMPro;
using Cysharp.Threading.Tasks;
using Lean.Gui;
using System.Drawing;
using static DataManager;


public class SpeedUpQuiz : MonoBehaviour
{
    // audio
    /*public AudioClip correct_audio;
    public AudioClip wrong_audio;
    public AudioClip question_audio;
    private AudioSource audioSource;*/

    // other
    //public GameObject play_canvas;
    //public GameObject question;
    //public GameObject[] buttons_obj;
    public GameObject score_obj;
    public GameObject ever_score_obj;
    //public GameObject skip_obj;
    public GameObject previous_answer_obj;

    //private List<LeanButton> buttons = new List<LeanButton>();
    //private int correct_index = 0;
    //private TextMeshProUGUI question_tmp;
    private TextMeshProUGUI score_tmp;
    private TextMeshProUGUI previous_answer_tmp;

    //private TextMeshProUGUI ever_score_tmp;
    //private List<TextMeshProUGUI> choices_tmp = new List<TextMeshProUGUI>();
    //private List<GameObject> correct_marks = new List<GameObject>();
    //private List<GameObject> wrong_marks = new List<GameObject>();
    private int question_index = 0;

    //正解数と出題数
    private int score = 0;
    private int quiz_count = 0;
    private int ever_score; //累積正解数
    private int ever_quiz_count;
    private int today_score;
    private int today_quiz_count;

    //フラグ
    private Boolean no_answer = true;
    private Boolean check_cancel = false;

    // change_dotsで使うフラグ
    private Boolean isCorrect = false;
    //private Boolean do_answer = false;
    private Boolean opening_scene = true;

    //クイズの中断
    public Boolean interruption = true;
    public Boolean interruption_by_setting = false;
    public Boolean interruption_by_status = false;

    //ストップボタン
    public GameObject stop_button_obj;
    public Image stopImage;
    public Image playImage;

    //下部GUIのキャンバス
    public Canvas setting_canvas;
    public Canvas status_canvas;

    // quiz interval
    private float interval_time;
    //public Image gaugeImage; // ゲージの画像
    private float elapsed_time = 0f; // ゲージ更新に関して

    // 注意 FindやGetComponentは優先順位の関係上Start()に入れないとnullが返されることがある
    public GameObject gameobject;
    Level_Decision Level_Decision_cs;

    //CancellationToken
    public CancellationTokenSource cts = new CancellationTokenSource();

    DataManager dataManager;
    Every_Play_Data every_play_data;
    Basic_Data basic_data;

    QuizManager quizManager;
    Quiz quiz;

    public GameObject statusObj;
    Scrollview_Object statusTmp;

    // リソースの獲得量
    public TextMeshProUGUI faithTmp;
    int numFaith;

    private void Awake()
    {
        every_play_data = new Every_Play_Data();
        every_play_data.type = "Speed_Up_Quiz";
    }

    async void Start()
    {
        quizManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<QuizManager>();
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();

        quizManager.ui.GenerateButtonsUI(4);

        LoadComponent();
        LoadData();

        quizManager.OnQuizServed += UpdateWhenQuizServed;
        quizManager.OnCorrect += WhenQuizCorrect;
        quizManager.OnWrong += WhenQuizWrong;

        //フラグの初期化
        no_answer = true;
        opening_scene = true;
        check_cancel = false;

        interruption = true;
        interruption_by_setting = false;
        interruption_by_status = false;

        stopImage.enabled = false;
        playImage.enabled = true;

        previous_answer_tmp.text = "←";

        //オブジェクト等の設定
        //skip_obj.SetActive(false);
        //play_canvas.GetComponent<Canvas>().enabled = true;

        //SetButtons();
        quizManager.ui.SetInteractable(false);
        //InteractableButtons(false);

        //オーディオを取得
        //audioSource = GetComponent<AudioSource>();

        //sceneUnloadedに関数を追加
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        //テキストの設定
        SetScoreText(score, quiz_count);
        SetResourceText(numFaith);

        await UniTask.WaitUntil(() => Level_Decision_cs.status); //Level_Decisionの初期化完了まで待機

        StartQuiz();
    }

    async void StartQuiz()
    {
        //EnabledButtons(true);
        quizManager.SetOptionsCount(1, 3);
        quizManager.ui.SetActive(true);
        quizManager.ui.SetEnabled(true);

        while (opening_scene) // 次の問題まで待つ
        {
            previous_answer_tmp.text = "← " + answer_text;

            //InteractableButtons(false);
            //ClearMarks();

            quizManager.ui.SetInteractable(false);
            quizManager.ui.HideMarks();

            await WaitNext();

            quizManager.GenMultiQuiz();
            interval_time = Level_Decision_cs.interval_time;

            isCorrect = false;
            await quizManager.SetQuiz(timeLimit: interval_time, isGauge: true, tapSkip: true);
            quiz = quizManager.GetQuiz();

            //elapsed_time = 0f;

            check_cancel = false;

            //await UniTask.Delay(TimeSpan.FromSeconds(interval_time / 10)); //誤操作防止のために少し待ってからボタンを押せるようにする
            //InteractableButtons(true);

            //do_answer = false;

            /*
            try
            {
                float seconds = interval_time - elapsed_time;
                print("interval_time :" + interval_time + " elapsed_time :" + elapsed_time + " seconds :" + seconds);
                await UniTask.Delay(TimeSpan.FromSeconds(Math.Max(seconds, 0.001f)), cancellationToken: cts.Token);
                Debug.Log("Delay completed.");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Delay cancelled.");
                check_cancel = true;
                cts.Dispose();
                cts = new CancellationTokenSource();
            }*/

            ChangeDots(quiz);
            answer_text = quiz.GetOptionsList()[quiz.GetCorrectIndexList()[0]];
            /*
            if (!do_answer)
            {
                isCorrect = false;
                //quizManager.IsCorrect(isCorrect);
            }
            do_answer = true;
            */

            //InteractableButtons(false);

            //skip_obj.SetActive(false);
            SetScoreText(score, quiz_count);

            statusTmp.AddItem(quiz.GetQuizSentence().ToString(), quiz.GetOptionsList()[quiz.GetCorrectIndexList()[0]].ToString(), isCorrect);
        }
    }

    async UniTask WaitNext()
    {
        while (interruption || interruption_by_setting || interruption_by_status)
        {
            interruption = true;
            stopImage.enabled = false;
            playImage.enabled = true;

            quizManager.ui.SetSentence("Tap <sprite name=play_button_asset> to Start");
            quizManager.ui.ClearOptionsText();
            
            //for (int i = 0; i < buttons.Count; i++)
            //    choices_tmp[i].text = "";

            await UniTask.WaitUntil(() => !interruption);
        }
    }

    void UpdateWhenQuizServed()
    {
        //print(correct_index);
        //print(correct_marks.Count);

        question_index++;
        ever_quiz_count++;
        today_quiz_count++;
        quiz_count++;

        every_play_data.quiz_count = quiz_count;
        dataManager.SaveEveryPlayData(every_play_data);
        basic_data.set("Ever_Quiz_Count", ever_quiz_count);
        dataManager.SaveBasicData(basic_data);
    }

    void WhenQuizCorrect()
    {
        print("OK");

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

        Level_Decision_cs.finish_level_change = false;

        Level_Decision_cs.ChangePoint(true);
        SetScoreText(score, quiz_count);
        SetResourceText(numFaith);

        isCorrect = true;
    }

    void WhenQuizWrong()
    {
        print("NG");

        Level_Decision_cs.finish_level_change = false;

        Level_Decision_cs.ChangePoint(false);
        SetScoreText(score, quiz_count);

        isCorrect = false;
    }

    void LoadComponent()
    {
        //コンポーネントの取得
        statusTmp = statusObj.GetComponent<Scrollview_Object>();
        Level_Decision_cs = gameobject.GetComponent<Level_Decision>();

        //question_tmp = question.GetComponent<TextMeshProUGUI>();
        score_tmp = score_obj.GetComponent<TextMeshProUGUI>();
        //ever_score_tmp = ever_score_obj.GetComponent<TextMeshProUGUI>();

        previous_answer_tmp = previous_answer_obj.GetComponent<TextMeshProUGUI>();
    }
    void LoadData()
    {
        //データの読み込み
        basic_data = dataManager.load_basic_data("Speed_Up_Quiz");
        ever_score = basic_data.get("Ever_Correct_Count");
        ever_quiz_count = basic_data.get("Ever_Quiz_Count");

        /*
        ever_score = PlayerPrefs.GetInt("Speed_Up_Quiz_Ever_Score", PlayerPrefs.GetInt("Ever_Score", 0));
        ever_quiz_count = PlayerPrefs.GetInt("Speed_Up_Quiz_Ever_Quiz_Count", PlayerPrefs.GetInt("Ever_Quiz_Count", ever_score));

        today_score = PlayerPrefs.GetInt(get_now_date("Speed_Up_Quiz_Day_Score"), 0);
        today_quiz_count = PlayerPrefs.GetInt(get_now_date("Speed_Up_Quiz_Day_Quiz_Count"), today_score);
        */
    }

    /*
    void SetButtons()
    {
        // 選択肢の生成
        for (int i = 0; i < buttons_obj.Length; i++)
        {
            choices_tmp.Add(buttons_obj[i].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>());
            correct_marks.Add(buttons_obj[i].transform.GetChild(5).gameObject);
            wrong_marks.Add(buttons_obj[i].transform.GetChild(6).gameObject);
            //print(correct_marks.Count);

            var press_index = i;
            buttons.Add(buttons_obj[i].GetComponent<LeanButton>());
            buttons[i].OnClick.AddListener(() => CheckAnswer(press_index));
        }
    }

    void InteractableButtons(Boolean isInteractable)
    {
        for (int i = 0; i < buttons.Count; i++) // 選択肢を生成した後にinteractableはfalseにする必要がある
            buttons[i].interactable = isInteractable;
    }

    void EnabledButtons(Boolean isEnabled)
    {
        for (int i = 0; i < buttons.Count; i++) // 選択肢を生成した後にinteractableはfalseにする必要がある
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

    async void ChangeDots(Quiz quiz)
    {
        if (!quiz.IsAnswered)
        {
            Level_Decision_cs.finish_level_change = false;
            //Level_Decision_cs.change_dots_score(-1);
            Level_Decision_cs.ChangePoint(false);
        }

        await UniTask.WaitUntil(() => Level_Decision_cs.finish_level_change);
    }

    string answer_text = "";

    /*
    async void SetQuiz()
    {
        await UniTask.WaitUntil(() => no_answer);

        //print(correct_index);
        //print(correct_marks.Count);

        quizManager.SetOptionsCount(1, 3);
        quiz = quizManager.GenMultiQuiz();
        audioSource.PlayOneShot(question_audio); // 効果音
        question_tmp.text = quiz.GetQuizSentence();
        correct_index = quiz.GetCorrectIndexList()[0];
        List<string> english_list = quiz.GetOptionsList();
        answer_text = quiz.GetOptionsList()[correct_index];

        for (int i = 0; i < english_list.Count; i++)
        {
            choices_tmp[i].text = english_list[i];
        }

        question_index++;
        ever_quiz_count++;
        today_quiz_count++;
        quiz_count++;

        every_play_data.quiz_count = quiz_count;
        every_play_data.save();
        basic_data.set("Ever_Quiz_Count", ever_quiz_count);
        basic_data.save();
    }

    async void CheckAnswer(int press_index)
    {
        if (do_answer == true) {
            return;
        }

        do_answer = true;
        InteractableButtons(false);

        no_answer = false;

        if (correct_index == press_index)
        {
            print("OK");
            audioSource.PlayOneShot(correct_audio);
            
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
            isCorrect = false;
        }

        SetScoreText(score, quiz_count);
        correct_marks[correct_index].SetActive(true);
        Level_Decision_cs.finish_level_change = false;
        no_answer = true;

        //quizManager.IsCorrect(isCorrect);
        
        if (isCorrect)
        {
            //Level_Decision_cs.change_dots_score(1);
            Level_Decision_cs.ChangePoint(true);
            if (interval_time - elapsed_time >= 0.5)
            {
                skip_obj.SetActive(true);
            }
        }
        else
        {
            //Level_Decision_cs.change_dots_score(-1);
            Level_Decision_cs.ChangePoint(false);
            if (interval_time - elapsed_time >= 2)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                skip_obj.SetActive(true);
            }
        }
    }*/

    private float fillAmount; // ゲージの現在の長さ（0から1の間の値）
    void Update()
    {
        //UpdateGauge();
        CheckInterraption();
    }

    void CheckInterraption()
    {
        interruption_by_setting = setting_canvas.enabled;
        interruption_by_status = status_canvas.enabled;
    }

    /*
    void UpdateGauge()
    {
        elapsed_time += Time.deltaTime;
        //if (!interruption && check_cancel)
        //    elapsed_time = interval_time;
        if(interval_time >= 1)
            fillAmount = 1 - (elapsed_time / (interval_time - (float)0.06)); // ゲージの長さを計算
        else
            fillAmount = 1 - (elapsed_time / (interval_time * (float)0.94)); // ゲージの長さを計算
        gaugeImage.fillAmount = fillAmount; // ゲージの長さを更新
    }*/

    void OnSceneUnloaded(Scene current)
    {
        print("OnSceneUnloaded: " + current);
        opening_scene = false;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void SetScoreText(int num_score, int num_quiz_count) {
        print(num_score + " " + num_quiz_count);
        float correct_ratio;
        if (num_quiz_count > 0)
            correct_ratio = (float)num_score / (float)num_quiz_count * 100;
        else
            correct_ratio = 0;
        score_tmp.text = num_quiz_count + "<size=24> 問中 </size>" + num_score + "<size=24> 問正解 (正解率 " + string.Format("{0:f1}", correct_ratio) + "%) </size>";
        //score_tmp.text = num_score + "<size=24> 問正解 </size>/<size=24> 累積 </size>" + num_ever_score + "<size=24> 問正解</size>";
        //ever_score_tmp.text = "<size=24>累積 </size>" + num_ever_score + "<size=24> 問正解 </size>";
    }

    public void stop_button_OnClick()
    {
        if (interruption)
        {
            interruption = false;
            stopImage.enabled = true;
            playImage.enabled = false;
        }
        else
        {
            interruption = true;
            stopImage.enabled = false;
            playImage.enabled = true;
        }
    }

    void SetResourceText(int num)
    {
        if (faithTmp != null)
        {
            faithTmp.text = "<sprite name=faith>" + num.ToString();
        }
    }

    public void SkipQuiz()
    {
        cts.Cancel();
    }
}
