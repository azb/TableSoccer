using Photon.Pun;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public PhotonView PossessingPlayer; //Which player currently possesses control over the ball
    Rigidbody rb;
    PhotonView photonView;
    ResetOnOutOfBounds resetter;

    public AudioClip KickBallSoundEffect;
    public AudioClip BallHitGoalSoundEffect;
    public AudioClip BallHitPlasticWallSoundEffect;
    public AudioSource AudioSource;

    public AudioClip AnnouncerGoal;

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
        if (collision.gameObject.tag == "Player")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PossessingPlayer = collision.gameObject.transform.GetComponent<PhotonView>();
                photonView.RPC("SetPossessingPlayer", RpcTarget.All, PossessingPlayer.ViewID);
            }
        }

        if (collision.gameObject.tag == "PlasticWall")
        {
            this.AudioSource.PlayOneShot(this.BallHitPlasticWallSoundEffect);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            this.AudioSource.PlayOneShot(this.BallHitGoalSoundEffect);
            if (PhotonNetwork.IsMasterClient)
            {
                Invoke("PlayAnnouncerGoalSound", .5f);
                photonView.RPC("ResetBall", RpcTarget.All);
                Debug.Log("GOAL!!!!!!");
                Goal goal = collision.gameObject.transform.GetComponent<Goal>();
                goal.OnGoal();
            }
        }
    }

    void PlayAnnouncerGoalSound()
    {
        photonView.RPC("PlayAnnouncerGoalSoundRPC", RpcTarget.All);
    }

    [PunRPC]
    void PlayAnnouncerGoalSoundRPC()
    {
        this.AudioSource.PlayOneShot(this.AnnouncerGoal);
    }

    [PunRPC]
    void ResetBall()
    {
        PossessingPlayer = null;
        resetter.ResetPosition();
    }
}