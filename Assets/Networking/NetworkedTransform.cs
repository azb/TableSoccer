using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedTransform : MonoBehaviour
{
    public float UpdatesPerSecond = 30;

    Vector3 positionPrevious;

    public bool SyncPosition;
    public bool SyncRotation;

    NetworkedObject _networkedObject;
    NetworkedObject networkedObject
    {
        get
        {
            if (_networkedObject == null)
            {
                _networkedObject = GetComponent<NetworkedObject>();
            }
            return _networkedObject;
        }
    }

    Vector3 prevPositionUpdate;

    // Start is called before the first frame update
    void Start()
    {
        prevPositionUpdate = transform.localPosition;
        if (networkedObject == null)
        {
            Debug.LogError("Networked object is null!");
        }
        networkedObject.SetSyncedVector3("transform.localPosition", transform.localPosition);
        networkedObject.SetSyncedVector3("transform.rotation", transform.rotation.eulerAngles);
        Invoke("UpdateTransform", 1f / UpdatesPerSecond);
    }

    private void UpdateTransform()
    {
        Vector3 positionPrevious = networkedObject.GetSyncedVector3("transform.localPosition");
        if (Vector3.Distance(positionPrevious, transform.localPosition) > .01f)
        {
            if (networkedObject.IsMine || (NetworkSync.IsMasterClient && networkedObject.OwnerID == -1))
            {
                networkedObject.SetSyncedVector3("transform.localPosition", transform.localPosition);
            }
        }

        Vector3 rotationPrevious = networkedObject.GetSyncedVector3("transform.rotation");
        if (Vector3.Distance(rotationPrevious, transform.rotation.eulerAngles) > .01f)
        {
            if (networkedObject.IsMine || (NetworkSync.IsMasterClient && networkedObject.OwnerID == -1))
            {
                networkedObject.SetSyncedVector3("transform.rotation", transform.rotation.eulerAngles);
            }
        }

        Invoke("UpdateTransform", 1f / UpdatesPerSecond);
    }

    private void Update()
    {
        if (!(networkedObject.IsMine || (NetworkSync.IsMasterClient && networkedObject.OwnerID == -1)))
        {
            if (SyncPosition)
            {
                Vector3 newPositionUpdate = networkedObject.GetSyncedVector3("transform.localPosition");
                if (Vector3.Distance(prevPositionUpdate, newPositionUpdate) > .01f)
                {
                    transform.localPosition += (newPositionUpdate - transform.localPosition)
                        * Mathf.Min(Time.deltaTime * 10f, 1f);
                }
            }

            if (SyncRotation)
            {
                Vector3 newRotationUpdate = networkedObject.GetSyncedVector3("transform.rotation");
                transform.rotation = Quaternion.Euler(newRotationUpdate);
            }
        }
    }
}
