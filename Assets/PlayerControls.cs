using UnityEngine;
using Photon.Pun;

public class PlayerControls : MonoBehaviour
{
    public float MoveSpeed = .1f;

    public GameObject AimArrow;

    NetworkedObject networkedObject;

    // Start is called before the first frame update
    void Start()
    {
        networkedObject = GetComponent<NetworkedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!networkedObject.IsMine)
            return;
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        float lookX = Input.GetAxis("Horizontal2");
        float lookY = Input.GetAxis("Vertical2");

#if UNITY_ANDROID //Meta Quest Controls
        // Check for joystick movement on the left controller
        Vector2 leftJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        // Check for joystick movement on the right controller
        Vector2 rightJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        moveX += leftJoystick.x;
        moveY += leftJoystick.y;

        lookX += rightJoystick.x;
        lookY += rightJoystick.y;
#endif

        //Debug.Log("Look Vector: ["+lookX+","+lookY+"]");

        if (Mathf.Abs(moveX) + Mathf.Abs(moveY) > .2f)
        {

            transform.position += new Vector3(
                moveX,
                0,
                moveY
                )
                * Time.deltaTime * MoveSpeed;

            if (Mathf.Abs(lookX) + Mathf.Abs(lookY) <= .2f)
            {
                float direction = -Mathf.Atan2(moveY, moveX) * Mathf.Rad2Deg + 90f;

                transform.rotation = Quaternion.Euler(0, direction, 0);
            }
            //Debug.Log("Move Vector: ["+moveX+","+moveY+"]");

        }

        if (Mathf.Abs(lookX) + Mathf.Abs(lookY) > .2f)
        {
            AimArrow.SetActive(true);
            float direction = -Mathf.Atan2(lookY, lookX) * Mathf.Rad2Deg + 180f;

#if UNITY_ANDROID && !UNITY_EDITOR //Meta Quest Controls
            direction -= 90f;
#endif

            transform.rotation = Quaternion.Euler(0, direction, 0);
        }
        else
        {
            AimArrow.SetActive(false);
        }
    }
}
