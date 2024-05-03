using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    public GameObject goalAnimation;
    public UnityEvent goalAction;

    // Start is called before the first frame update
    void Start()
    {
        HideGoalAnimation();
    }

    // Update is called once per frame
    public void OnGoal()
    {
        goalAnimation.SetActive(true);
        goalAction.Invoke();
        Invoke("HideGoalAnimation", 5f);
    }

    void HideGoalAnimation()
    {
        goalAnimation.SetActive(false);
    }
}
