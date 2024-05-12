using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerControls : MonoBehaviour
{
    public GameObject kickModel;
    public GameObject runModel1;
    public GameObject runModel2;
    public GameObject standModel;

    public float MoveSpeed = .1f;

    public GameObject AimArrow;

    PhotonView photonView;

    public AudioSource audioSource;
    public AudioClip RunningOnGrass;

    [Range (0,1)]
    public float moveSoundEffectVolume = .5f;

    //QUEST
    public string MoveXAxisQuest = "JoystickAxis1";
    public string MoveYAxisQuest = "JoystickAxis2";
    public string LookXAxisQuest = "JoystickAxis3";
    public string LookYAxisQuest = "JoystickAxis4";
    public string KickAxisQuest = "JoystickAxis5";
    public bool InvertMoveXQuest = false;
    public bool InvertMoveYQuest = false;
    public bool InvertLookXQuest = false;
    public bool InvertLookYQuest = false;

    //VISION OS
    public string MoveXAxisVisionOS = "JoystickAxis1";
    public string MoveYAxisVisionOS = "JoystickAxis2";
    public string LookXAxisVisionOS = "JoystickAxis3";
    public string LookYAxisVisionOS = "JoystickAxis4";
    public string KickAxisVisionOS = "JoystickAxis5";
    public bool InvertMoveXVisionOS = false;
    public bool InvertMoveYVisionOS = false;
    public bool InvertLookXVisionOS = false;
    public bool InvertLookYVisionOS = false;

    //EDITOR
    public string MoveXAxisEditor = "JoystickAxis9";
    public string MoveYAxisEditor = "JoystickAxis10";
    public string LookXAxisEditor = "JoystickAxis3";
    public string LookYAxisEditor = "JoystickAxis4";
    public string KickAxisEditor = "JoystickAxis5";
    public bool InvertMoveXEditor = false;
    public bool InvertMoveYEditor = false;
    public bool InvertLookXEditor = false;
    public bool InvertLookYEditor = false;

    public static Dictionary<string, string> controls = new Dictionary<string, string>();
    public static Dictionary<string, bool> invertControls = new Dictionary<string, bool>();
    public static Dictionary<string, float> neutralValue = new Dictionary<string, float>();

    private void Awake()
    {
        if (controls.ContainsKey("MoveX"))
            return;
        controls.Add("MoveX", "JoystickAxis1");
        controls.Add("MoveY", "JoystickAxis2");
        controls.Add("AimX", "JoystickAxis3");
        controls.Add("AimY", "JoystickAxis4");
        controls.Add("Kick", "joystick 1 button 1");

        invertControls.Add("MoveX", false);
        invertControls.Add("MoveY", false);
        invertControls.Add("AimX", false);
        invertControls.Add("AimY", false);
        invertControls.Add("Kick", false);

        neutralValue.Add("MoveX", 0);
        neutralValue.Add("MoveY", 0);
        neutralValue.Add("AimX", 0);
        neutralValue.Add("AimY", 0);
        neutralValue.Add("Kick", 0);
    }

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


    public bool kicking;


    void DoneKicking()
    {
        kicking = false;
        runModel1.SetActive(false);
        runModel2.SetActive(false);
        standModel.SetActive(true);
        kickModel.SetActive(false);
    }

    [PunRPC]
    public void Kick()
    {
        Debug.Log("Kick");
        kicking = true;
        runModel1.SetActive(false);
        runModel2.SetActive(false);
        standModel.SetActive(false);
        kickModel.SetActive(true);
        Invoke("DoneKicking", .2f);
    }

    bool running;
    Vector3 prevPosition;
    float timeSincleLastRun = 0;

    public static bool KickButtonHeld = false;

    public static bool KickButtonPressed
    {
        get
        {
            bool kickButtonPressed = false;

#if UNITY_ANDROID && META_QUEST //Meta Quest Controls
            // Check if the trigger button on the left controller is pressed
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                //Debug.Log("Left trigger pressed");
                return true;
            }

            // Check if the trigger button on the right controller is pressed
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                //Debug.Log("Right trigger pressed");
                return true;
            }
#endif

            if (PlayerControls.controls["Kick"] != null)
            {
                string kickInput = PlayerControls.controls["Kick"];
                if (kickInput.Contains("button"))
                {
                    if (!KickButtonHeld && Input.GetButton(kickInput))
                    {
                        kickButtonPressed = Input.GetButton(kickInput);
                        if (Input.GetButton(kickInput))
                        {
                            KickButtonHeld = true;
                        }
                    }
                    else
                    {
                        kickButtonPressed = false;
                        if (!Input.GetButton(kickInput))
                        {
                            KickButtonHeld = false;
                        }
                    }
                }
                else if (kickInput.Contains("Axis"))
                {
                    kickButtonPressed = Mathf.Abs(Input.GetAxis(kickInput) - PlayerControls.neutralValue["Kick"]) > .2f;
                }
            }

            return kickButtonPressed || Input.GetKeyDown(KeyCode.Space);
        }
    }

    bool canKick = true;

    // Update is called once per frame
    void Update()
    {
        if (!kicking)
        {
            this.kickModel.SetActive(false);
            this.runModel1.SetActive(running && side == 1);
            this.runModel2.SetActive(running && side == 2);
            this.standModel.SetActive(!running);
        }

        if (!photonView.IsMine)
        {
            if (Vector3.Distance(transform.position , prevPosition) > 0)
            {
                prevPosition = transform.position;
                running = true;
                timeSincleLastRun = 0;
            }
            else
            {
                timeSincleLastRun += Time.deltaTime;
                if (timeSincleLastRun > .05)
                {
                    running = false;
                }
            }

            return;
        }

        if (PlayerControls.KickButtonPressed)
        {
            if (!kicking && canKick)
            {
                canKick = false;
                kicking = true;
                photonView.RPC("Kick", RpcTarget.All);
            }
            canKick = false;
        }
        else
        {
            canKick = true;
        }

        float moveX = 0;
        float moveY = 0;
        float lookX = 0;
        float lookY = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveX -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveX += 1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveY += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveY -= 1;
        }

        moveX -= Input.GetAxis(controls["MoveX"]);
        moveY += Input.GetAxis(controls["MoveY"]);
        
        if (invertControls["MoveX"])
        {
            moveX *= -1;
        }

        if (invertControls["MoveY"])
        {
            moveY *= -1;
        }

        lookX += Input.GetAxis(controls["AimX"]);
        lookY += Input.GetAxis(controls["AimY"]);

        if (invertControls["AimX"])
        {
            lookX *= -1;
        }

        if (invertControls["AimY"])
        {
            lookY *= -1;
        }


#if UNITY_ANDROID && META_QUEST //Meta Quest Controls
        
        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            // Check for joystick movement on the left controller
            Vector2 leftJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

            // Check for joystick movement on the right controller
            Vector2 rightJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

            moveX = leftJoystick.x;
            moveY = leftJoystick.y;

            lookY = -rightJoystick.x;
            lookX = -rightJoystick.y;
        }    
#endif

        if (Mathf.Abs(moveX) + Mathf.Abs(moveY) > .2f)
        {
            audioSource.volume = Mathf.Clamp( 
                Mathf.Sqrt(moveX * moveX + moveY * moveY) , 
                0 , 
                1
            ) * moveSoundEffectVolume;

            running = true;

            Vector3 moveVector = new Vector3(
                moveX,
                0,
                moveY
                )
                * Time.deltaTime * MoveSpeed;

#if UNITY_ANDROID && META_QUEST //Meta Quest Controls
            float angle = Camera.main.transform.rotation.eulerAngles.y;
#else
            float angle = 0;
#endif

            // Create a quaternion rotation
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

            // Rotate the vector using quaternion multiplication
            Vector3 rotatedVector = rotation * moveVector;


            transform.position += rotatedVector;

            // If not aiming with aim thumbstick,
            // set the direction to movement direction
            if (Mathf.Abs(lookX) + Mathf.Abs(lookY) <= .2f)
            {
                float direction = -Mathf.Atan2(moveY, moveX) * Mathf.Rad2Deg - 90f;

                transform.rotation = Quaternion.Euler(0, direction + 180, 0);
            }
        }
        else
        {
            audioSource.volume = 0;
            running = false;
        }

        //If aiming with aim thumbstick
        if (Mathf.Abs(lookX) + Mathf.Abs(lookY) >= .2f)
        {
            AimArrow.SetActive(true);
            float direction = Mathf.Atan2(lookY, lookX) * Mathf.Rad2Deg - 90f;

#if UNITY_ANDROID && !UNITY_EDITOR //Meta Quest Controls
            direction -= 90f;
#endif

            transform.rotation = Quaternion.Euler(0, direction, 0);
        }
        else
        {
            AimArrow.SetActive(false);
        }
        
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        x = Mathf.Clamp(x, -.27f, .27f);
        z = Mathf.Clamp(z, -.43f, .43f);

        transform.position = new Vector3(
                x,
                y,
                z
            );
    }
}
