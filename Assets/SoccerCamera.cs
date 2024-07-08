using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerCamera : MonoBehaviour
{
    Transform ball;

    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindWithTag("SoccerBall").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (ball.position - transform.position) * Time.deltaTime;
    }
}
