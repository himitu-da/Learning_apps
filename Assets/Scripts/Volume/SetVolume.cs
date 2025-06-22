using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using Lean.Transition.Method;
using Cysharp.Threading.Tasks;

public class SetVolume : MonoBehaviour
{
    public List<GameObject> gameObjects;
    
    public List<LeanPlaySound> playSounds;
    public AudioSource quizAudio;

    private float quizVolume, buttonVolume;
    private readonly string quizKey = "Volume_Quiz", buttonKey = "Volume_Button";


    void Start()
    {
        quizVolume = PlayerPrefs.GetFloat(quizKey, 1f);
        buttonVolume = PlayerPrefs.GetFloat(buttonKey, 1f);

        /*foreach (GameObject go in gameObjects)
        {
            foreach(LeanPlaySound sound in go.GetComponentsInChildren<LeanPlaySound>())
            {
                playSounds.Add(sound);
            }
        }*/

        SetSounds();
    }

    async public void SetSounds()
    {
        await UniTask.Yield();
        // クイズのコンポーネントを取得
        quizAudio = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<AudioSource>();

        LeanPlaySound[] sounds = FindObjectsOfType<LeanPlaySound>();

        // ボタンのコンポーネントを取得

        foreach (LeanPlaySound sound in sounds)
        {
            if (!playSounds.Contains(sound))
                playSounds.Add(sound);
        }

        SetQuizVolume(quizVolume);
        SetButtonVolume(buttonVolume);

        print("changed volume");
    }

    public void ChangeQuizSlider(float volume)
    {
        quizVolume = volume;
        SetQuizVolume(quizVolume);
    }

    public void ChangeButtonSlider(float volume)
    {
        buttonVolume = volume;
        SetButtonVolume(buttonVolume);
    }

    void SetQuizVolume(float volume)
    {
        quizAudio.volume = volume * volume;
    }

    void SetButtonVolume(float volume)
    {
        foreach (LeanPlaySound playSound in playSounds)
        {
            playSound.Data.Volume = volume * volume;
        }
    }
}
