using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float MoveSpeed = .1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        transform.position += new Vector3(
            moveX,
            0,
            moveY
            )
            * Time.deltaTime * MoveSpeed;

        float direction = -Mathf.Atan2(moveY, moveX) * Mathf.Rad2Deg + 90f;

        transform.rotation = Quaternion.Euler(0, direction, 0);

        Debug.Log("Move Vector: ["+moveX+","+moveY+"]");
    }
}
