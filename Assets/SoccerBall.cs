using Photon.Pun;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public PhotonView PossessingPlayer; //Which player currently possesses control over the ball
    Rigidbody rb;
    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PossessingPlayer || !PossessingPlayer.IsMine)
            return;

        transform.position = PossessingPlayer.transform.position + PossessingPlayer.transform.forward * .01f + Vector3.up * .01f;

        bool KickButtonPressed = (Input.GetAxis("Fire1") > .1f);


#if UNITY_ANDROID //Meta Quest Controls
        // Check if the trigger button on the left controller is pressed
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            //Debug.Log("Left trigger pressed");
            KickButtonPressed = true;
        }

        // Check if the trigger button on the right controller is pressed
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            //Debug.Log("Right trigger pressed");
            KickButtonPressed = true;
        }
#endif

        if (KickButtonPressed)
        {
            photonView.RPC("KickButtonPressedRPC", RpcTarget.All);
            Debug.Log("Fire button pressed");

        }
    }

    [PunRPC]
    void KickButtonPressedRPC()
    {
        Debug.Log("KickButtonPressedRPC");
        //Kick the ball
        rb.velocity = PossessingPlayer.transform.forward * .5f + Vector3.up;
        PossessingPlayer = null;
    }

    [PunRPC]
    void SetPossessingPlayer(int possessingPlayerViewID)
    {
        PossessingPlayer = PhotonView.Find(possessingPlayerViewID);
        Debug.Log("SetPossessingPlayer to "+ PossessingPlayer.ViewID);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (collision.gameObject.tag == "Player")
        {
            PossessingPlayer = collision.gameObject.transform.GetComponent<PhotonView>();
            photonView.RPC("SetPossessingPlayer", RpcTarget.All, PossessingPlayer.ViewID);
        }
    }
}
