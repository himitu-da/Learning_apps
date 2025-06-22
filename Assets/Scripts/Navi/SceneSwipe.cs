using Cysharp.Threading.Tasks;
using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwipe : MonoBehaviour
{
    public int sceneRightIndex, sceneLeftIndex;
    string[] sceneList = CONSTANTS.NAVISCENELIST;
    private async void Start()
    {
        LeanDrag drag = gameObject.GetComponent<LeanDrag>();
        LeanConstrainAnchoredPosition lean = gameObject.GetComponent<LeanConstrainAnchoredPosition>();

        drag.interactable = false;
        await UniTask.Delay(75);
        drag.interactable = true;

        if (sceneLeftIndex == -1)
        {
            lean.HorizontalPixelMax = 0;
        }
        if (sceneRightIndex == -1)
        {
            lean.HorizontalPixelMin = 0;
        }
    }

    public void OnChanged(Vector2Int vector)
    {
        print(vector);
        if (vector.x == 1)
        {
            SceneManager.LoadScene(sceneList[sceneLeftIndex]);
        } else if (vector.x == -1)
        {
            SceneManager.LoadScene(sceneList[sceneRightIndex]);
        }
    }
}
