using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public RectTransform ProgressBarForeground;

    public float progress = 0;
    public float maxProgress = 100;

    // Start is called before the first frame update
    void Start()
    {

        Vector2 sizeDelta = ProgressBarForeground.sizeDelta;
        sizeDelta.x = 0;
        ProgressBarForeground.sizeDelta = sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 sizeDelta = ProgressBarForeground.sizeDelta;
        sizeDelta.x = progress / maxProgress * 600;
        ProgressBarForeground.sizeDelta = sizeDelta;

    }
}
