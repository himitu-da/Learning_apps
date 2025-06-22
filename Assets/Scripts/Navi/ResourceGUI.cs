using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceGUI : MonoBehaviour
{
    DataManager dataManager;
    public TextMeshProUGUI faith, knowledge, sunpower;

    void Start()
    {
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();
        
        if (faith != null)
            dataManager.res.SetText("Faith", faith);
        if (knowledge != null)
            dataManager.res.SetText("Knowledge", knowledge);
        if (sunpower != null)
            dataManager.res.SetText("SunPower", sunpower);
        }

    void Update()
    {
        if (faith != null)
            dataManager.res.SetText("Faith", faith);
        if (knowledge != null)
            dataManager.res.SetText("Knowledge", knowledge);
        if (sunpower != null)
            dataManager.res.SetText("SunPower", sunpower);
    }
}
