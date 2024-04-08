using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GameObject goalAnimation;

    // Start is called before the first frame update
    void Start()
    {
        HideGoalAnimation();
    }

    // Update is called once per frame
    public void OnGoal()
    {
        goalAnimation.SetActive(true);
        Invoke("HideGoalAnimation", 5f);
    }

    void HideGoalAnimation()
    {
        goalAnimation.SetActive(false);
    }
}
