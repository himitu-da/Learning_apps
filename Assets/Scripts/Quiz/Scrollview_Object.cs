using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Scrollview_Object : MonoBehaviour
{
    public GameObject content; // スクロールビューのContentオブジェクト
    public GameObject prefab;
    GameObject newItem;
    List<GameObject> stack = new List<GameObject>();

    public void AddItem(string text, string text2, bool isCorrect)
    {
        if (stack.Count < 50)
        {
            newItem = Instantiate(prefab);
        }
        else
        {
            newItem = stack[0];
            stack.RemoveAt(0);
        }

        // テキストを設定（アイテムがTextまたはButtonの場合）
        TextMeshProUGUI[] itemText = newItem.GetComponentsInChildren<TextMeshProUGUI>();

        itemText[0].text = text;
        itemText[1].text = text2;

        if (isCorrect)
        {
            itemText[2].enabled = true;
            itemText[3].enabled = false;
        } else
        {
            itemText[2].enabled = false;
            itemText[3].enabled = true;
        }

        newItem.transform.SetParent(content.transform, false);
        newItem.transform.SetSiblingIndex(0);

        if (content.transform.childCount >= 0)
        {
            //GameObject old = content.transform.GetChild(content.transform.childCount - 1).gameObject;
            stack.Add(newItem);
        }
    }
}