using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetButtonToList : MonoBehaviour
{
    void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("EventSystem");
        SetVolume buttonVolume =  obj.GetComponent<SetVolume>();

        buttonVolume.gameObjects.Add(gameObject);
    }

}
