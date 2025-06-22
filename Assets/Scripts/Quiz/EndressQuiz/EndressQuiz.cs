using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System ;
using TMPro;
using Cysharp.Threading.Tasks;
using Lean.Gui;
using System.Drawing;
using static DataManager;
using JetBrains.Annotations;

public class EndressQuiz : MonoBehaviour
{
    // audio
    //public AudioClip correct_audio;
    //public AudioClip wrong_audio;
    //public AudioClip question_audio;
    //private AudioSource audioSource;

    // other
    //public GameObject play_canvas;
    //public GameObject question;
    //public GameObject[] buttons_obj;
    public GameObject score_obj;
    public GameObject previous_answer_obj;

    //private List<LeanButton> buttons = new List<LeanButton>();
    //private int correct_index = 0;
    //private TextMeshProUGUI question_tmp;
    private TextMeshProUGUI score_tmp;
    private TextMeshProUGUI previous_answer_tmp;

    //private List<TextMeshProUGUI> choices_tmp = new List<TextMeshProUGUI>();
    //private List<GameObject> correct_marks = new List<GameObject>();
    //private List<GameObject> wrong_marks = new List<GameObject>();

    private int question_index = 0;
    private int score = 0;
    private int quiz_count = 0;
    private int ever_score; //累積正解数
    private int ever_quiz_count;
    private int today_score;
    private int today_quiz_count;

    //private float elapsed_time = 0f;
    private Boolean opening_scene;
    //private Boolean do_answer;
    private Boolean isCorrect;

    //クイズの中断
    public Boolean interruption = true;
    public Boolean interruption_by_setting = false;
    public Boolean interruption_by_status = false;

    //下部GUIのキャンバス
    public Canvas setting_canvas;
    public Canvas status_canvas;

    //ストップボタン
    public GameObject stop_button_obj;
    public Image stopImage;
    public Image playImage;

    // quiz interval
    public static int interval_time = 4000;
    private static float float_interval_time = (float)(interval_time / 1000);
    public Image gaugeImage; // ゲージの画像
    private float bar_time = 0f; // ゲージ更新に関して

    DataManager dataManager;
    Every_Play_Data every_play_data;
    Basic_Data basic_data;

    QuizManager quizManager;
    Quiz quiz;

    public GameObject statusObj;
    Scrollview_Object statusTmp;

    // リソースの獲得量
    public TextMeshProUGUI faithTmp;
    int numFaith = 0;

    void Awake()
    {
        every_play_data = new Every_Play_Data();
        every_play_data.type = "Endress_Quiz";
    }
    void Start()
    {
        quizManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<QuizManager>();
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();

        quizManager.ui.GenerateButtonsUI(4);

        LoadComponent();
        LoadData();

        quizManager.OnQuizServed += UpdateWhenQuizServed;
        quizManager.OnCorrect += WhenQuizCorrect;
        quizManager.OnWrong += WhenQuizWrong;

        //初期化
        opening_scene = true;
        interruption = true;
        interruption_by_setting = false;
        interruption_by_status = false;
        stopImage.enabled = false;
        playImage.enabled = true;

        SceneManager.sceneUnloaded += OnSceneUnloaded;  //sceneUnloadedに関数を追加
        
        previous_answer_tmp.text = "←";

        //play_canvas.GetComponent<Canvas>().enabled = true;
        
        //SetButtons();
        SetScoreText(score, quiz_count);
        SetResourceText(numFaith);
        
        //audioSource = GetComponent<AudioSource>(); // オーディオを取得
        StartQuiz();
    }

    async void StartQuiz()
    {
        //EnabledButtons(true);
        quizManager.ui.SetActive(true);
        quizManager.ui.SetEnabled(true);

        quizManager.SetOptionsCount(1, 3);

        while (opening_scene) // 次の問題まで待つ
        {
            previous_answer_tmp.text = "← " + answerText;

            //InteractableButtons(false);
            //ClearMarks();

            quizManager.ui.SetInteractable(false);
            quizManager.ui.HideMarks();
            await WaitNext();
            quizManager.GenMultiQuiz();
            bar_time = (float)GetNextInterval() / 1000;

            isCorrect = false;
            await quizManager.SetQuiz(timeLimit: bar_time, isGauge: true);
            quiz = quizManager.GetQuiz();
            answerText = quiz.GetOptionsList()[quiz.GetCorrectIndexList()[0]];

            /*
            //bar_time = float_interval_time;
            //elapsed_time = 0f;

            //await UniTask.Delay(TimeSpan.FromMilliseconds(interval_time / 10));
            //InteractableButtons(true);

            //do_answer = false;

            //await UniTask.Delay(TimeSpan.FromMilliseconds(GetNextInterval()));

            if (!do_answer)
            {
                isCorrect = false;
                quizManager.IsCorrect(isCorrect);
            }
            do_answer = true;
            */

            //InteractableButtons(false);
            SetScoreText(score, quiz_count);

            statusTmp.AddItem(quiz.GetQuizSentence().ToString(), quiz.GetOptionsList()[quiz.GetCorrectIndexList()[0]].ToString(), isCorrect);
        }
    }

    async UniTask WaitNext()
    {
        while (interruption || interruption_by_setting || interruption_by_status) //ストップ中か設定を開いているとき
        {
            interruption = true;
            stopImage.enabled = false;
            playImage.enabled = true;
            
            quizManager.ui.SetSentence("Tap <sprite name=play_button_asset> to Start");
            quizManager.ui.ClearOptionsText();

            await UniTask.WaitUntil(() => !interruption);

            quizManager.ui.SetSentence("Please Wait...");
            bar_time = (float)GetNextInterval() / 1000;

            quizManager.ResetTimeCount();
            await quizManager.TimeCount(bar_time);
            //elapsed_time = 0f;
            //await UniTask.Delay(TimeSpan.FromMilliseconds(GetNextInterval()));
        }
    }

    void LoadComponent()
    {
        //コンポーネントの取得
        statusTmp = statusObj.GetComponent<Scrollview_Object>();

        //play_canvas.GetComponent<Canvas>().enabled = false;
        //question_tmp = question.GetComponent<TextMeshProUGUI>();
        score_tmp = score_obj.GetComponent<TextMeshProUGUI>();
        previous_answer_tmp = previous_answer_obj.GetComponent<TextMeshProUGUI>();
    }

    void LoadData()
    {
        basic_data = dataManager.load_basic_data("Endress_Quiz");
        ever_score = basic_data.get("Ever_Correct_Count");
        ever_quiz_count = basic_data.get("Ever_Quiz_Count");
        //データの読み込み

        /*
        ever_score = PlayerPrefs.GetInt("Endress_Question_Ever_Score", PlayerPrefs.GetInt("Ever_Score_Endress_Question", 0));
        ever_quiz_count = PlayerPrefs.GetInt("Endress_Question_Ever_Quiz_Count", PlayerPrefs.GetInt("Ever_Quiz_Count_Endress_Question", ever_score));

        today_score = PlayerPrefs.GetInt(get_now_date("Endress_Question_Day_Score"), 0);
        today_quiz_count = PlayerPrefs.GetInt(get_now_date("Endress_Question_Day_Quiz_Count"), today_score);
        */
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

        SetScoreText(score, quiz_count);
        SetResourceText(numFaith);

        isCorrect = true;
    }

    void WhenQuizWrong()
    {
        print("NG");

        SetScoreText(score, quiz_count);

        isCorrect= false;
    }

    /*
    void SetButtons()
    {
        for (int i = 0; i < buttons_obj.Length; i++) // 選択肢の生成
        {

            choices_tmp.Add(buttons_obj[i].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>());
            correct_marks.Add(buttons_obj[i].transform.GetChild(5).gameObject);
            wrong_marks.Add(buttons_obj[i].transform.GetChild(6).gameObject);

            var press_index = i;
            buttons.Add(buttons_obj[i].GetComponent<LeanButton>());
            buttons[i].OnClick.AddListener(() => CheckAnswer(press_index));
        }
    }

    void InteractableButtons(Boolean isInteractable){
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
    }
    */

    int GetNextInterval()
    {
        DateTime now_time = DateTime.Now;
        int now_milisec = now_time.Millisecond;
        int now_second = now_time.Second * 1000;
        int now_minute = now_time.Minute * 1000 * 60;
        int now_mili = now_milisec + now_second + now_minute;
        int until_next_interval = (interval_time) - now_mili % interval_time;
        return until_next_interval;
    }

    string answerText = "";

    /*
    void SetQuiz()
    {
        print(correct_index);
        print(correct_marks.Count);

        quizManager.SetOptionsCount(1, 3);
        quiz = quizManager.GenMultiQuiz();
        audioSource.PlayOneShot(question_audio); // 効果音
        question_tmp.text = quiz.GetQuizSentence();
        correct_index = quiz.GetCorrectIndexList()[0];
        List<string> english_list = quiz.GetOptionsList();
        
        answerText = quiz.GetOptionsList()[correct_index];

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

    void CheckAnswer(int press_index)
    {
        //if (elapsed_time <= interval_time / 5 && elapsed_time <= 0.4)
        //    return;
        
        if (do_answer == true)
            return;

        do_answer = true;
        InteractableButtons(false);

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

        correct_marks[correct_index].SetActive(true);
        //quizManager.IsCorrect(isCorrect);
        SetScoreText(score, quiz_count);
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
        fillAmount = 1 - elapsed_time / bar_time; // ゲージの長さを計算
        gaugeImage.fillAmount = fillAmount; // ゲージの長さを更新
    }*/

    void OnSceneUnloaded(Scene current)
    {
        print("OnSceneUnloaded: " + current);
        opening_scene = false;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void SetScoreText(int num_score, int num_quiz_count)
    {
        float correct_ratio;
        if (num_quiz_count > 0)
            correct_ratio = (float)num_score / (float)num_quiz_count * 100;
        else
            correct_ratio = 0;
        score_tmp.text = num_quiz_count + "<size=24> 問中 </size>" + num_score + "<size=24> 問正解 (正解率 " + string.Format("{0:f1}", correct_ratio) + "%) </size>";
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
}
