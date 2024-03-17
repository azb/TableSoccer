using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnOutOfBounds : MonoBehaviour
{
    Vector3 startPosition;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "OutOfBounds")
        {
            transform.position = startPosition;
            rb.velocity = Vector3.zero;
        }
    }
}
