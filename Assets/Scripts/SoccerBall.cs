using Photon.Pun;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public PhotonView LastPossessingPlayer;
    public PhotonView _possessingPlayer;
    public PhotonView PossessingPlayer //Which player currently possesses control over the ball
    {
        get
        {
            return _possessingPlayer;
        }
        set
        {
            LastPossessingPlayer = _possessingPlayer;
            _possessingPlayer = value;
        }
    }
    Rigidbody rb;
    PhotonView photonView;
    ResetOnOutOfBounds resetter;

    public AudioClip KickBallSoundEffect;
    public AudioClip BallHitGoalSoundEffect;
    public AudioClip BallHitPlasticWallSoundEffect;
    public AudioClip OutOfBoundsSoundEffect;
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
        CheckForRepossession(collision);

        if (collision.gameObject.tag == "PlasticWall")
        {
            this.AudioSource.PlayOneShot(this.BallHitPlasticWallSoundEffect);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckForRepossession(collision);
    }

    void CheckForRepossession(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            float radius = .02f;
            if (Vector3.Distance(transform.position, collision.transform.position + collision.transform.forward * radius / 2f) < radius)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PossessingPlayer = collision.gameObject.transform.GetComponent<PhotonView>();
                    photonView.RPC("SetPossessingPlayer", RpcTarget.All, PossessingPlayer.ViewID);
                }
            }
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
                SoccerGame.Instance.gameState = SoccerGame.GameState.Goal;
            }
        }

        if (collision.gameObject.tag == "OutOfBounds")
        {
            this.AudioSource.PlayOneShot(this.OutOfBoundsSoundEffect);
            SoccerGame.SetState(SoccerGame.GameState.BallOutOfBounds);
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
        Invoke("StartPlaying", 5f);
    }

    void StartPlaying()
    {
        SoccerGame.Instance.gameState = SoccerGame.GameState.Playing;
    }
}