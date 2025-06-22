using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;

public class ChangeQuizVolume : MonoBehaviour
{
    private float volume;
    private readonly string key = "Volume_Quiz";

    private GameObject sliderObj;
    private Slider slider;

    private SetVolume setVolume;
    private string tag = "EventSystem";


    // Start is called before the first frame update
    void Start()
    {
        volume = PlayerPrefs.GetFloat(key, 1f);
        slider = GetComponent<Slider>();
        slider.value = volume;

        setVolume = GameObject.FindGameObjectWithTag(tag).GetComponent<SetVolume>();
    }

    public void ChangeSlider()
    {
        PlayerPrefs.SetFloat(key, slider.value);
        if (setVolume != null)
           setVolume.ChangeQuizSlider(slider.value);
    }
}
