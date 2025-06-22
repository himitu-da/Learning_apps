using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.EventSystems;
using System.Threading;
using Cysharp.Threading.Tasks.Triggers;

public class QuizManager : MonoBehaviour
{
    AudioSource audioSource;

    List<Quiz> quizList = new List<Quiz>();

    public QuizUIManager ui;
    QuizDatabase quizDatabase;
    
    List<string> optionsList = new List<string>();
    List<int> correctIndexList = new List<int>();
    List<List<string>> optionsListUnshuffle = new List<List<string>>();

    CancellationTokenSource cts = new();

    public event Action OnQuizServed;
    public event Action OnCorrect;
    public event Action OnWrong;

    float elapsedTime;

    int correctOptionsCount, wrongOptionsCount;
    bool canClick = false, tapSkip = false;

    void Awake()
    {
        ui = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).AddComponent<QuizUIManager>();
        quizDatabase = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).AddComponent<QuizDatabase>();

        ui.OnSkipped += () => Cancel();
        ui.OnAnswered += CheckAnswer;
        audioSource = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<AudioSource>();
    }

    void Cancel()
    {
        cts.Cancel();
    }

    async public UniTask<bool> SetQuiz(int index = -1, float timeLimit = 0f, bool isGauge = false, bool tapSkip = false)
    {
        if (index == -1) // 指定のない場合は最新のクイズを指定するということ
            index = quizList.Count - 1;
        if (timeLimit == 0f)
        {
            isGauge = false;
            tapSkip = false;
        }
        this.tapSkip = tapSkip;
        // 0.5秒 or 30%の時間 は押せなくする

        ui.HideMarks();

        ui.SetInteractable(false);

        Quiz quiz;
        quiz = quizList[index];
        quiz.TimeLimit = timeLimit;

        ResetTimeCount();
        TimeCount(quiz.TimeLimit, isGauge);

        ui.SetQuizUI(quiz);
        audioSource.PlayOneShot(ui.serveAudio);
        OnQuizServed();
        quiz.IsAnswered = false;

        if (quiz.TimeLimit <= 0f)
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        else
            await UniTask.Delay(TimeSpan.FromSeconds(Math.Min(0.5f, quiz.TimeLimit * 0.3f)));

        ui.SetInteractable(true);
        canClick = true;

        try
        {
            if (quiz.TimeLimit <= 0f)
                await UniTask.WaitUntil(() => quiz.IsAnswered);
            else if (tapSkip)
            {
                await UniTask.WaitUntil(() => elapsedTime >= quiz.TimeLimit, cancellationToken: cts.Token);
            }
            else
                await UniTask.WaitUntil(() => elapsedTime >= quiz.TimeLimit);
        }
        catch (OperationCanceledException)
        {
            print("Cancelled");
            cts.Dispose();
            cts = new();
        }
        ui.SetSkip(false);

        canClick = false;

        if (!quiz.IsAnswered)
        {
            quiz.IsCorrect = false;
            quizDatabase.DoChangeIsCorrect(false);
        }
        ui.SetInteractable(false);

        return quiz.IsCorrect;
    }

    public async void CheckAnswer(int pressIndex, Quiz quiz)
    {
        if (quiz.IsAnswered || !canClick)
            return;

        quiz.IsAnswered = true;
        ui.SetInteractable(false);

        int correctIndex = quizList[quizList.Count - 1].GetCorrectIndexList()[0];
        if (pressIndex == correctIndex)
        {
            print("OK");
            ui.ShowCorrectMark(pressIndex);
            audioSource.PlayOneShot(ui.correctAudio);
            quiz.IsCorrect = true;
            OnCorrect();
            quizDatabase.DoChangeIsCorrect(true);

            if (quiz.TimeLimit - elapsedTime > 0.5f && tapSkip)
                ui.SetSkip(true);
        }
        else
        {
            print("NG");
            ui.ShowWrongMark(pressIndex);
            ui.ShowCorrectMark(correctIndex);
            audioSource.PlayOneShot(ui.wrongAudio);
            quiz.IsCorrect = false;
            OnWrong();
            quizDatabase.DoChangeIsCorrect(false);

            if (quiz.TimeLimit - elapsedTime > 2f && tapSkip)
            {
                await UniTask.Delay(1000);
                ui.SetSkip(true);
            }
    }
    }

    public void ResetTimeCount()
    {
        elapsedTime = 0f;
    }

    public async UniTask TimeCount(float untilTime, bool isGauge = true)
    {
        if (untilTime == 0f)
            return;
        float fillAmount;

        while (elapsedTime < untilTime && !cts.IsCancellationRequested)
        {
            await UniTask.Yield();
            elapsedTime += Time.deltaTime;

            if (isGauge)
            {
                if (untilTime >= 1)
                    fillAmount = 1 - (elapsedTime / (untilTime - (float)0.06)); // ゲージの長さを計算
                else
                    fillAmount = 1 - (elapsedTime / (untilTime * (float)0.94)); // ゲージの長さを計算
                ui.timeGauge.fillAmount = fillAmount; // ゲージの長さを更新
            }
        }
        ui.timeGauge.fillAmount = 0f;
    }

    public void SetOptionsCount(int correctOptionsCount, int wrongOptionsCount)
    {
        this.correctOptionsCount = correctOptionsCount;
        this.wrongOptionsCount = wrongOptionsCount;
    }

    public Quiz GenMultiQuiz()
    {
        (var correctOptionsList, var wrongOptionsList) = quizDatabase.GetListSet(correctOptionsCount, wrongOptionsCount);

        int quizNumber = quizList.Count + 1;
        string quizType = "MultiChoice";
        string quizSentence = correctOptionsList[0][2];
        optionsList.Clear();
        correctIndexList.Clear();
        optionsListUnshuffle.Clear();

        optionsListUnshuffle.AddRange(correctOptionsList);
        optionsListUnshuffle.AddRange(wrongOptionsList);

        for (int i = 0; i < correctOptionsCount + wrongOptionsCount; i++)
        {
            int num = UnityEngine.Random.Range(0, optionsListUnshuffle.Count);
            if (optionsListUnshuffle[num][0] == "Correct")
            {
                correctIndexList.Add(i); //何番目に追加されたかが答えのインデックス
            }
            optionsList.Add(optionsListUnshuffle[num][1]);
            optionsListUnshuffle.RemoveAt(num);
        }

        Quiz quiz = new Quiz(quizNumber, quizType, quizSentence, optionsList, correctIndexList);

        quizList.Add(quiz);
        return quiz;
    }

    public Quiz GetQuiz(int index = -1)
    {
        if (index < 0)
        {
            return quizList[quizList.Count + index];
        }
        else
        {
            return quizList[index];
        }
    }

    public List<Quiz> GetQuizList()
    {
        return quizList;
    }

    public int GetDatabaseCount()
    {
        return quizDatabase.GetDatabaseCount();
    }

    
}
