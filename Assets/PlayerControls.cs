using UnityEngine;
using Photon.Pun;

public class PlayerControls : MonoBehaviour
{
    public GameObject kickModel;
    public GameObject runModel1;
    public GameObject runModel2;
    public GameObject standModel;

    public float MoveSpeed = .1f;

    public GameObject AimArrow;

    PhotonView photonView;

    public string MoveXAxisQuest = "JoystickAxis1";
    public string MoveYAxisQuest = "JoystickAxis2";
    public string LookXAxisQuest = "JoystickAxis3";
    public string LookYAxisQuest = "JoystickAxis4";
    public string KickAxisQuest = "JoystickAxis5";
    public bool InvertMoveXQuest = false;
    public bool InvertMoveYQuest = false;
    public bool InvertLookXQuest = false;
    public bool InvertLookYQuest = false;


    public string MoveXAxisVisionOS = "JoystickAxis1";
    public string MoveYAxisVisionOS = "JoystickAxis2";
    public string LookXAxisVisionOS = "JoystickAxis3";
    public string LookYAxisVisionOS = "JoystickAxis4";
    public string KickAxisVisionOS = "JoystickAxis5";
    public bool InvertMoveXVisionOS = false;
    public bool InvertMoveYVisionOS = false;
    public bool InvertLookXVisionOS = false;
    public bool InvertLookYVisionOS = false;


    public string MoveXAxisEditor = "JoystickAxis9";
    public string MoveYAxisEditor = "JoystickAxis10";
    public string LookXAxisEditor = "JoystickAxis3";
    public string LookYAxisEditor = "JoystickAxis4";
    public string KickAxisEditor = "JoystickAxis5";
    public bool InvertMoveXEditor = false;
    public bool InvertMoveYEditor = false;
    public bool InvertLookXEditor = false;
    public bool InvertLookYEditor = false;

    public string KickButton = "joystick 1 button 1";

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        Invoke("SwitchSide",.2f);
    }

    int side = 1;

    void SwitchSide()
    {
        if (side == 1)
        {
            side = 2;
        }
        else
        {
            side = 1;
        }

        Invoke("SwitchSide", .2f);
    }


    bool kicking;


    void DoneKicking()
    {
        kicking = false;
        runModel1.SetActive(false);
        runModel2.SetActive(false);
        standModel.SetActive(true);
        kickModel.SetActive(false);
    }

    bool running;

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
            return;

        bool KickButtonPressed = Input.GetButton("joystick 1 button 1");

        if (KickButtonPressed)
        {
            kicking = true;
            runModel1.SetActive(false);
            runModel2.SetActive(false);
            standModel.SetActive(false);
            kickModel.SetActive(true);
            Invoke("DoneKicking",.2f);
        }

        float moveX = 0;
        float moveY = 0;
        float lookX = 0;
        float lookY = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("LeftArrow");
            moveX -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("RightArrow");
            moveX += 1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("UpArrow");
            moveY -= 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("DownArrow");
            moveY += 1;
        }


        // Check the platform and assign appropriate input axis names
        if (Application.isEditor)
        {
            moveX += Input.GetAxis(MoveXAxisEditor);
            moveY += Input.GetAxis(MoveYAxisEditor);

            if (InvertMoveXEditor)
            {
                moveX *= -1;
            }

            if (InvertMoveYEditor)
            {
                moveY *= -1;
            }

            lookX += Input.GetAxis(KickAxisEditor);
            lookY += Input.GetAxis(KickAxisEditor);

            if (InvertLookXEditor)
            {
                lookX *= -1;
            }

            if (InvertLookYEditor)
            {
                lookY *= -1;
            }
        }
        else
        if (Application.platform == RuntimePlatform.Android)
        {
            moveX += Input.GetAxis(MoveXAxisQuest);
            moveY += Input.GetAxis(MoveYAxisQuest);

            if (InvertMoveXQuest)
            {
                moveX *= -1;
            }

            if (InvertMoveYQuest)
            {
                moveY *= -1;
            }

            lookX += Input.GetAxis(KickAxisQuest);
            lookY += Input.GetAxis(KickAxisQuest);

            if (InvertLookXQuest)
            {
                lookX *= -1;
            }

            if (InvertLookYQuest)
            {
                lookY *= -1;
            }

        }
        else //if (Application.platform == RuntimePlatform.VisionOS)
        {
            moveX = Input.GetAxis(MoveXAxisVisionOS);
            moveY = Input.GetAxis(MoveYAxisVisionOS);

            if (InvertMoveXVisionOS)
            {
                moveX *= -1;
            }

            if (InvertMoveYVisionOS)
            {
                moveY *= -1;
            }

            lookX += -Input.GetAxis(LookXAxisVisionOS);
            lookY += Input.GetAxis(LookYAxisVisionOS);

            if (InvertLookXVisionOS)
            {
                lookX *= -1;
            }

            if (InvertLookYVisionOS)
            {
                lookY *= -1;
            }
        }

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
            running = true;

            transform.position += new Vector3(
                moveX,
                0,
                moveY
                )
                * Time.deltaTime * MoveSpeed;

            if (Mathf.Abs(lookX) + Mathf.Abs(lookY) <= .2f)
            {
                float direction = -Mathf.Atan2(moveY, moveX) * Mathf.Rad2Deg - 90f;

                transform.rotation = Quaternion.Euler(0, direction, 0);
            }
            //Debug.Log("Move Vector: ["+moveX+","+moveY+"]");
        }
        else
        {
            running = false;
        }

        if (!kicking)
        {
            this.kickModel.SetActive(false);
            this.runModel1.SetActive(running && side == 1);
            this.runModel2.SetActive(running && side == 2);
            this.standModel.SetActive(!running);
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
