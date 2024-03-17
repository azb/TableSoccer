using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnOutOfBounds : MonoBehaviour
{
    Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "OutOfBounds")
        {
            transform.position = startPosition;
        }
    }
}
