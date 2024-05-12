using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    public GameObject goalAnimation;
    public UnityEvent goalAction;
    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        HideGoalAnimation();
    }

    // Update is called once per frame
    public void OnGoal()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ShowGoalAnimation", RpcTarget.All);
        }
    }

    [PunRPC]
    void ShowGoalAnimation()
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
