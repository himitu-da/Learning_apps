using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class SkipSystem : MonoBehaviour
{
    SpeedUpQuiz TQ_cs; //Tyming_Question
    // Start is called before the first frame update
    public void OnClick()
    {
        print("Skipped");
        TQ_cs = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<SpeedUpQuiz>();
        TQ_cs.SkipQuiz();
    }
}
