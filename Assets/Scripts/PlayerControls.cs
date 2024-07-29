using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using static SoccerGame;

public class PlayerControls : MonoBehaviour
{
    public float AISpeedScalar = .75f;
    public float AIKickDelay = .5f;

    public TeamID teamID;

    public GameObject kickModel;
    public GameObject runModel1;
    public GameObject runModel2;
    public GameObject standModel;

    public enum PlayerPosition { Goalie, Defense, Offense };
    public PlayerPosition playerPosition;

    public float MoveSpeed = .1f;

    public GameObject AimArrow;

    PhotonView photonView;

    public AudioSource audioSource;
    public AudioClip RunningOnGrass;

    [Range(0, 1)]
    public float moveSoundEffectVolume = .5f;

    public Material[] teamColorMaterials;

    public MeshRenderer[] body;

    float zoffset = 0;

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

    float possessionTime;

    private void Awake()
    {
        Invoke("ChooseNewZoffset", Random.value * 2f + .1f);

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

    void ChooseNewZoffset()
    {
        zoffset = Random.value * .1f;
        Invoke("ChooseNewZoffset", Random.value * 2f + .1f);
    }

    public string KickButton = "joystick 1 button 1";

    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        photonView = GetComponent<PhotonView>();
        Invoke("SwitchSide", .2f);
        /*
        for(int i = 0 ; i < body.Length ; i++)
        {
            body[i].material = this.teamColorMaterials[ (photonView.OwnerActorNr-1) % 2 ];
        }*/
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

    void DoKick()
    {
        if (!kicking && canKick)
        {
            kicking = true;
            if (PhotonNetwork.OfflineMode)
            {
                Kick();
            }
            else
            {
                photonView.RPC("Kick", RpcTarget.All);
            }
        }
        if (isHuman)
        {
            canKick = false;
        }
        else if (canKick)
        {
            canKick = false;
            Invoke("ReEnableKick", 1f);
        }
    }
    void ReEnableKick()
    {
        canKick = true;
    }

    public bool isHuman = true;
    // Update is called once per frame
    void Update()
    {
        if (SoccerGame.PossessingPlayer == this)
        {
            possessionTime += Time.deltaTime;
        }
        else
        {
            possessionTime = 0;
        }


        isHuman = true;
        if (SoccerGame.Instance.closestTeam1Player != this && this.teamID == TeamID.Team1)
            isHuman = false;
        if (SoccerGame.Instance.closestTeam2Player != this && this.teamID == TeamID.Team2)
            isHuman = false;
        if (photonView.OwnerActorNr != (int)this.teamID)
            isHuman = false;
        if (SoccerGame.GetState() != SoccerGame.GameState.Playing)
        {
            isHuman = false;
        }

        if (!kicking)
        {
            this.kickModel.SetActive(false);
            this.runModel1.SetActive(running && side == 1);
            this.runModel2.SetActive(running && side == 2);
            this.standModel.SetActive(!running);
        }

        if (!photonView.IsMine)
        {
            if (Vector3.Distance(transform.position, prevPosition) > 0)
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

            Debug.Log("returning because not my photon view");

            return;
        }

        float moveX = 0;
        float moveY = 0;
        float lookX = 0;
        float lookY = 0;

        if (isHuman)
        {
            //Human player controls

            if (PlayerControls.KickButtonPressed)
            {
                DoKick();
            }
            else
            {
                canKick = true;
            }


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
        }
        else
        {
            float distanceFromStartX = Mathf.Abs(transform.position.x - startPos.x);

            if (distanceFromStartX > .01f)
            {
                if (transform.position.x > startPos.x)
                {
                    moveX = -1;
                }

                if (transform.position.x < startPos.x)
                {
                    moveX = 1;
                }
            }

            float targetX = startPos.x;
            float targetZ = startPos.z;

            if (SoccerGame.Instance == null)
            {
                Debug.Log("Returning because SoccerGame is null");
                return;
            }
            if (SoccerGame.Instance.soccerBall == null)
            {
                Debug.Log("Returning because soccerball is null");
                return;
            }

            if (SoccerGame.GetState() == GameState.Playing)
            {
                if (SoccerGame.PossessingPlayer == this)
                {
                    //If soccer ball is on team 1's side and this player is on team 1 and a defender
                    if (SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team1Side
                        && this.teamID == TeamID.Team1
                        && this.playerPosition == PlayerPosition.Defense)
                    {
                        float centerZ = SoccerGame.Instance.transform.position.z;
                        targetZ = centerZ;
                        if (Mathf.Abs(targetZ - centerZ) < .02f)
                        {
                            //kick the ball
                            if (possessionTime > AIKickDelay)
                            {
                                this.DoKick();
                            }
                        }
                    }

                    //If soccer ball is on team 2's side and this player is on team 2 and a defender
                    if (SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team2Side
                        && this.teamID == TeamID.Team2
                        && this.playerPosition == PlayerPosition.Defense)
                    {
                        Debug.Log("A player 2 defensive player has the ball");

                        float centerZ = SoccerGame.Instance.transform.position.z;
                        targetZ = centerZ;
                        if (Mathf.Abs(targetZ - centerZ) < .02f)
                        {
                            Debug.Log("Should be kicking the ball");
                            //kick the ball
                            if (possessionTime > AIKickDelay)
                            {
                                this.DoKick();
                            }
                        }
                        else
                        {
                            Debug.Log("Should be running to the center");
                        }
                    }

                    //If this player is on team 1 and an attacker
                    if (this.teamID == TeamID.Team1
                        && this.playerPosition == PlayerPosition.Offense)
                    {
                        Debug.Log("A player 1 attacking player has the ball, should be running to team1Goal");

                        targetX = SoccerGame.Instance.team2Goal.position.x;
                        targetZ = SoccerGame.Instance.team2Goal.position.z;

                        if (Vector3.Distance(transform.position, new Vector3(targetX, transform.position.y, targetZ)) < .25f)
                        {
                            Debug.Log("In range so should be kicking!");
                            if (possessionTime > AIKickDelay)
                            {
                                DoKick();
                            }
                        }
                    }

                    //If this player is on team 2 and an attacker
                    if (this.teamID == TeamID.Team2
                        && this.playerPosition == PlayerPosition.Offense)
                    {
                        Debug.Log("A player 2 attacking player has the ball, should be running to team2Goal");

                        targetX = SoccerGame.Instance.team1Goal.position.x;
                        targetZ = SoccerGame.Instance.team1Goal.position.z;

                        if (Vector3.Distance(transform.position, new Vector3(targetX, transform.position.y, targetZ)) < .25f)
                        {
                            Debug.Log("In range so should be kicking!");
                            if (possessionTime > AIKickDelay)
                            {
                                DoKick();
                            }
                        }
                    }
                }
                else //If this player doesn't possess the ball
                {
                    //If soccer ball is on team 1's side and this player is on team 1 and a defender
                    if (SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team1Side
                        && this.teamID == TeamID.Team1
                        && this.playerPosition == PlayerPosition.Defense)
                    {
                        targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
                    }

                    //If soccer ball is on team 2's side and this player is on team 2 and a defender
                    if (SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team2Side
                        && this.teamID == TeamID.Team2
                        && this.playerPosition == PlayerPosition.Defense)
                    {
                        targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
                    }

                    //If soccer ball is on team 2's side or midfield and this player is on team 1 and an attacker
                    if ((SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team2Side
                        || SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Midfield)
                        && this.teamID == TeamID.Team1
                        && this.playerPosition == PlayerPosition.Offense)
                    {
                        targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
                    }

                    //If soccer ball is on team 1's side or midfield and this player is on team 2 and an attacker
                    if ((SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team1Side
                        || SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Midfield)
                        && this.teamID == TeamID.Team2
                        && this.playerPosition == PlayerPosition.Offense)
                    {
                        targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
                    }

                }
            }

            if (SoccerGame.GetState() == SoccerGame.GameState.Playing && !isHuman && this.playerPosition != PlayerPosition.Goalie)
            {
                //targetZ += zoffset;
            }

            if (SoccerGame.GetState() == SoccerGame.GameState.Playing)
            {
                if (SoccerGame.Instance.closestTeam1Player == this
                    && this.teamID == TeamID.Team1
                    && SoccerGame.PossessingPlayer != this)
                {
                    targetX = SoccerGame.Instance.soccerBall.transform.position.x;
                    targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
                }

                if (SoccerGame.Instance.closestTeam2Player == this
                    && this.teamID == TeamID.Team2
                    && SoccerGame.PossessingPlayer != this)
                {
                    targetX = SoccerGame.Instance.soccerBall.transform.position.x;
                    targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
                }
            }

            if (SoccerGame.GetState() == SoccerGame.GameState.Goal)
            {
                targetX = startPos.x;
                targetZ = startPos.z;
            }

            if (SoccerGame.GetState() == SoccerGame.GameState.BallOutOfBounds)
            {
                targetX = startPos.x;
                targetZ = startPos.z;

                PhotonView LastPossessingPlayerPhotonView = SoccerGame.Instance.soccerBall.LastPossessingPlayer;
                if (LastPossessingPlayerPhotonView != null)
                {
                    PlayerControls LastPossessingPlayer = LastPossessingPlayerPhotonView.GetComponent<PlayerControls>();

                    if (LastPossessingPlayer != null)
                    {
                        if (SoccerGame.Instance.soccerBall.teamThatKickedTheBallOutOfBounds != teamID) //if the other team kicked the ball out of bounds
                        {
                            if (teamID == TeamID.Team1)
                            {
                                if (SoccerGame.Instance.closestTeam1Player == this) //if this is the closest player
                                {
                                    //if not possessing the ball, go get it
                                    if (SoccerGame.Instance.soccerBall.PossessingPlayer != this.photonView)
                                    {
                                        targetX = SoccerGame.Instance.soccerBall.transform.position.x;
                                        targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
                                    }
                                    else
                                    {
                                        //once possessing the ball, go to the nearest sideline kick position

                                    }
                                }
                            }
                            else //if on team 2
                            {
                                if (SoccerGame.Instance.closestTeam2Player == this) //if this is the closest player
                                {
                                    //if not possessing the ball, go get it
                                    if (SoccerGame.Instance.soccerBall.PossessingPlayer != this.photonView)
                                    {
                                        targetX = SoccerGame.Instance.soccerBall.transform.position.x;
                                        targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
                                    }
                                    else
                                    {
                                        //once possessing the ball, go to the nearest sideline kick position
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }

            float distanceFromTargetX = Mathf.Abs(transform.position.x - targetX);
            if (distanceFromTargetX > .01f && !isHuman)
            {
                if (transform.position.x > targetX)
                {
                    moveX = -1;
                }

                if (transform.position.x < targetX)
                {
                    moveX = 1;
                }
            }

            float distanceFromTargetZ = Mathf.Abs(transform.position.z - targetZ);
            if (distanceFromTargetZ > .01f && !isHuman)
            {
                if (transform.position.z > targetZ)
                {
                    moveY = -1;
                }

                if (transform.position.z < targetZ)
                {
                    moveY = 1;
                }
            }
        }

        if (!isHuman)
        {
            moveX *= AISpeedScalar;
            moveY *= AISpeedScalar;
        }

        if (Mathf.Abs(moveX) + Mathf.Abs(moveY) > .2f)
        {
            audioSource.volume = Mathf.Clamp(
                Mathf.Sqrt(moveX * moveX + moveY * moveY),
                0,
                1
            ) * moveSoundEffectVolume;

            running = true;

            Vector3 moveVector = new Vector3(
                moveX,
                0,
                moveY
                )
                * Time.deltaTime * MoveSpeed;

#if (UNITY_ANDROID && META_QUEST) || UNITY_EDITOR //Meta Quest Controls
            float angle = Camera.main.transform.rotation.eulerAngles.y;
            //Debug.Log("angle = "+angle);
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

                transform.rotation = Quaternion.Euler(0, direction + 180 + angle, 0);
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
