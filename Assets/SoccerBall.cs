using Photon.Pun;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public Transform PossessingPlayer; //Which player currently possesses control over the ball
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PossessingPlayer)
        {
            transform.position = PossessingPlayer.position + PossessingPlayer.forward * .01f + Vector3.up * .01f;

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
                Debug.Log("Fire button pressed");
                //Kick the ball
                rb.velocity = PossessingPlayer.forward * .5f + Vector3.up;
                PossessingPlayer = null;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (collision.gameObject.tag == "Player")
        {
            PossessingPlayer = collision.gameObject.transform;
        }
    }
}
