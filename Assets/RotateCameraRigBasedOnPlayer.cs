using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RotateCameraRigBasedOnPlayer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        }
    }

}
