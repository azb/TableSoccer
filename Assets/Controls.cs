using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        if (Mathf.Abs(moveX) > .1f)
        {
            transform.position += new Vector3(moveX, 0, 0) * Time.deltaTime;
        }
        if (Mathf.Abs(moveY) > .1f)
        {
            transform.position += new Vector3(0, 0, moveY) * Time.deltaTime;
        }
    }
}
