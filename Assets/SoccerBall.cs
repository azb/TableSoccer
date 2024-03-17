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
            if (Input.GetAxis("Fire1") > .1f)
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
        if (collision.gameObject.tag == "Player")
        {
            PossessingPlayer = collision.gameObject.transform;
        }
    }
}
