using Photon.Pun;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public PhotonView PossessingPlayer; //Which player currently possesses control over the ball
    Rigidbody rb;
    PhotonView photonView;
    ResetOnOutOfBounds resetter;

    public AudioClip KickBallSoundEffect;
    public AudioSource AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        resetter = GetComponent<ResetOnOutOfBounds>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PossessingPlayer)
            return;

        transform.position = PossessingPlayer.transform.position + PossessingPlayer.transform.forward * .01f + Vector3.up * .005f;

        if (!PossessingPlayer.IsMine)
            return;

#if UNITY_ANDROID && META_QUEST //Meta Quest Controls
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

        if (PossessingPlayer.GetComponent<PlayerControls>().kicking == true)
        {
            photonView.RPC("KickButtonPressedRPC", RpcTarget.All);
            Debug.Log("Fire button pressed");
        }
    }

    [PunRPC]
    void KickButtonPressedRPC()
    {
        if (PossessingPlayer!=null)
        {
            Debug.Log("KickButtonPressedRPC");
            //Kick the ball
            rb.velocity = PossessingPlayer.transform.forward * .5f + Vector3.up;
            PossessingPlayer = null;
            this.AudioSource.PlayOneShot(this.KickBallSoundEffect);
        }
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

    private void OnTriggerEnter(Collider collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("GOAL!!!!!!");
            Goal goal = collision.gameObject.transform.GetComponent<Goal>();
            goal.OnGoal();
            photonView.RPC("ResetBall", RpcTarget.All);
        }
    }

    [PunRPC]
    void ResetBall()
    {
        resetter.ResetPosition();
    }
}