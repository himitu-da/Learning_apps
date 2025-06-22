using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ResponsiveUI : MonoBehaviour
{
    public RectTransform uiElement;
    public float default_height = 140f; // �Œ荂��
    public float max_height_percentage = 0.1f;
    public float min_height_percentage = 0.05f; 
    private float height;
    private float width;

    void Start()
    {
        height = Screen.height;
        width = Screen.width;
        AdjustSize(height, width);
    }

    void Update()
    {
        if(Screen.height != height || Screen.width != width)
        {
            height = Screen.height;
            width = Screen.width;
            print(height + ";" + width);
            AdjustSize(height, width); // ��ʃT�C�Y���ύX���ꂽ�Ƃ��ɑΉ�
        }
    }

    void AdjustSize(float h, float w)
    {
        float max_height = h * max_height_percentage;
        float min_height = h * min_height_percentage;
        // �傫������I��
        float finalHeight = median(max_height, min_height, default_height);
        // ������ݒ�
        uiElement.sizeDelta = new Vector2(w, finalHeight);
        uiElement.anchoredPosition = new Vector2(0, finalHeight / 2);
    }

    float median(float a, float b, float c)
    {
        if((a >= b && a <= c) || (a <= b && a >= c))
        {
            return a;
        } else if((b >= a && b <= c) || (b <= a) && (b >= c))
        {
            return b;
        } else
        {
            return c;
        }
    }
}
