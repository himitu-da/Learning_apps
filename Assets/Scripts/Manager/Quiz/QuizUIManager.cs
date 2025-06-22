using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Lean.Gui;
using System;

public class QuizUIManager : MonoBehaviour
{
    int optionsCount = 4;
    public Image prehabTimeGauge, timeGauge;
    public AudioClip correctAudio, wrongAudio, serveAudio;

    TextMeshProUGUI questionTmp;
    List<TextMeshProUGUI> optionsTmp = new();
    GameObject prehabButtons, buttons;
    GameObject prehabTapSkip, tapSkip;
    List<GameObject> options = new(), corrects = new(), wrongs = new();
    List<LeanButton> leanButtons = new();

    Quiz quiz;

    public event Action<int, Quiz> OnAnswered;
    public event Action OnSkipped;

    public QuizUIManager()
    {

    }

    void Awake()
    {
        correctAudio = Resources.Load<AudioClip>("Audio/CorrectSE");
        wrongAudio = Resources.Load<AudioClip>("Audio/WrongSE");
        serveAudio = Resources.Load<AudioClip>("Audio/ServeSE");
        prehabTimeGauge = Resources.Load<GameObject>("Prehabs/ElapsedGauge").GetComponent<Image>();
        timeGauge = Instantiate(prehabTimeGauge, GameObject.Find("StatusCanvas").transform);
        timeGauge.transform.SetSiblingIndex(0);
        timeGauge.fillAmount = 0f;

        prehabTapSkip = Resources.Load<GameObject>("Prehabs/TapSkip");
        tapSkip = Instantiate(prehabTapSkip);
        
        LeanButton l = tapSkip.transform.Find("skip").gameObject.GetComponent<LeanButton>();
        l.OnClick.AddListener(() => OnClickSkip());

    }

    public void GenerateButtonsUI(int optionsCount)
    {
        this.optionsCount = optionsCount;

        switch (optionsCount)
        {
            case 4:
                prehabButtons = Resources.Load<GameObject>("Prehabs/Buttons/4Buttons");
                break;
            default:
                prehabButtons = Resources.Load<GameObject>("Prehabs/Buttons/4Buttons");
                break;
        }

        buttons = Instantiate(prehabButtons);

        buttons.SetActive(false);

        questionTmp = buttons.transform.Find("Question").GetComponent<TextMeshProUGUI>();

        options.Clear();
        corrects.Clear();
        wrongs.Clear();
        leanButtons.Clear();

        for (int i = 0; i < optionsCount; i++)
        {
            GameObject obj = buttons.transform.Find("Canvas").Find("Button" + (i + 1).ToString()).gameObject;
            LeanButton leanButton = obj.GetComponent<LeanButton>();
            options.Add(obj);
            leanButtons.Add(leanButton);
            int pressIndex = i;
            leanButton.OnClick.AddListener(() => OnAnswered(pressIndex, quiz));
        }

        foreach(GameObject g in options)
        {
            optionsTmp.Add(g.transform.Find("Cap").GetComponentInChildren<TextMeshProUGUI>());
            corrects.Add(g.transform.Find("Correct").gameObject);
            wrongs.Add(g.transform.Find("Wrong").gameObject);
        }

    }

    public void SetQuizUI(Quiz quiz)
    {
        questionTmp.text = quiz.GetQuizSentence();

        for(int i = 0; i < optionsTmp.Count; i++)
        {
            optionsTmp[i].text = quiz.GetOptionsList()[i];
        }

        this.quiz = quiz;
    }

    public void SetActive(bool isActive)
    {
        buttons.SetActive(isActive);
        GameObject.FindGameObjectWithTag("EventSystem").GetComponent<SetVolume>().SetSounds();
    }

    public void SetInteractable(bool isInteractable)
    {
        foreach (LeanButton lean in leanButtons)
        {
            lean.interactable = isInteractable;
        }
    }

    public void SetEnabled(bool isEnabled)
    {
        foreach (LeanButton lean in leanButtons)
        {
            lean.enabled = isEnabled;
        }
    }

    public void HideMarks()
    {
        foreach (GameObject correct in corrects)
            correct.SetActive(false);
        foreach (GameObject wrong in wrongs)
            wrong.SetActive(false);
    }

    public void ShowCorrectMark(int index)
    {
        corrects[index].SetActive(true);
    }

    public void ShowWrongMark(int index)
    {
        wrongs[index].SetActive(true);
    }

    public void SetSentence(string sentence)
    {
        questionTmp.text = sentence;
    }

    public void ClearOptionsText()
    {
        foreach (TextMeshProUGUI text in optionsTmp)
            text.text = "";
    }

    public void SetSkip(bool isActive)
    {
        tapSkip.SetActive(isActive);
    }

    public void OnClickSkip()
    {
        print("OnClicked");
        OnSkipped();
    }

    public void SetOptionsCount(int optionsCount)
    {
        this.optionsCount = optionsCount;
        GenerateButtonsUI(optionsCount);
    }

    public int GetOptionsCount()
    {
        return this.optionsCount;
    }
}
