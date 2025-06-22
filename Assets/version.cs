using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class version : MonoBehaviour
{
    public TextMeshProUGUI te;

    // Start is called before the first frame update
    void Start()
    {
        string version = Application.version;
        te.text = "version " + version;
        }
    }