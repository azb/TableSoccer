using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using System;
using System.IO;

public class ControllerCalibrationUI : MonoBehaviour
{
    public TextMeshProUGUI selectedInputLabel;
    public TextMeshProUGUI requestedInputLabel;

    string[] requestedInputLabels =
    {
        "WaitingForController",
        "Neutral",
        "Move Left",
        "Move Forward",
        "Aim Left",
        "Aim Forward",
        "Kick"
        //"Move Right",
        //"Move Backward",
        //"Aim Right",
        //"Aim Backward",
    };

    string[] requestedInputActionName =
    {
        "WaitingForController",
        "Neutral",
        "MoveX",
        "MoveY",
        "AimX",
        "AimY",
        "Kick"
    };

    public Animator GameControllerAnimator;

    public GameObject TopLevelPrefab;

    public ProgressBar progressBar;
    public GameObject GreenCheckMark;

    public GameObject CalibrateNeutralPanel;

    public GameObject CalibrateInputPanel;

    public GameObject CalibrationCompletePanel;

    public GameObject ControllerNotFoundPanel;
    public GameObject ControllerDisconnectedNotification;

    public GameObject MoveRightPanel;
    public GameObject MoveUpPanel;
    public GameObject MoveDownPanel;

    public GameObject AimLeftPanel;
    public GameObject AimRightPanel;
    public GameObject AimUpPanel;
    public GameObject AimDownPanel;

    public GameObject KickScreen;
    public GameObject CompleteScreen;

    public string MoveXAxis = "";
    public string MoveYAxis = "";
    public string LookXAxis = "";
    public string LookYAxis = "";
    public string KickButton = "";

    public GameObject[] tutorialGameObjects;

    CalibrationScreen calibrationScreen = CalibrationScreen.ConnectAController;

    public enum CalibrationScreen {
        ConnectAController,
        CalibrateNeutral,
        MoveLeftScreen,
        MoveRightScreen,
        MoveUpScreen,
        MoveDownScreen,
        AimLeftScreen,
        AimRightScreen,
        AimUpScreen,
        AimDownScreen,
        KickScreen,
        CompleteScreen
    };

    public float[] axisDefaultValue = new float[13];
    bool[] buttonDefaultValue = new bool[20];

    bool stepCompleted = false;
    bool configCompleted = false;

    PlayerControls playerControls;

    void CheckForController()
    {
        string[] joystickNames = Input.GetJoystickNames();

        if (calibrationScreen == CalibrationScreen.ConnectAController)
        {
            Debug.Log("Checking for controller joystickNames.Length = "+ joystickNames.Length);
            
            if (joystickNames.Length > 0)
            {
                Debug.Log("Game controller found with name "+ joystickNames[0]);
                //If controller is found, go to calibrate neutral screen
                ControllerNotFoundPanel.SetActive(false);
                CalibrateNeutralPanel.SetActive(true);
                calibrationScreen = CalibrationScreen.CalibrateNeutral;
                progressBar.gameObject.SetActive(true);
            }
            else
            {
                //If no controller is found, prompt user to connect a controller
                ControllerNotFoundPanel.SetActive(true);
                CalibrateNeutralPanel.SetActive(false);
                progressBar.gameObject.SetActive(false);
                //Periodically check if a controller was connected until one is connected
            }
        }
        else
        {
            ControllerDisconnectedNotification.SetActive(joystickNames.Length == 0);
        }

        Invoke("CheckForController", 3f);
    }

    void Start()
    {
        CheckForController();

        string controlsFileName = Application.persistentDataPath + "/Controls.csv";
        Debug.Log("controlsFileName = "+ controlsFileName);
        if (File.Exists("Controls.csv"))
        {
            File.OpenRead("Controls.csv");

            string controls = File.ReadAllText("Controls.csv");

            Debug.Log("Controls.csv contents = "+controls);
        }
        else
        {
            Debug.Log("Controls.csv not found");
        }

        Invoke("CheckForPlayer", 1f);
        GreenCheckMark.SetActive(false);

        for (int i = 1; i <= 12; i++)
        {
            axisDefaultValue[i] = Input.GetAxis("JoystickAxis" + i);
        }

        for (int i = 0; i < 20; i++)
        {
            buttonDefaultValue[i] = Input.GetButton("joystick 1 button " + i);
        }

        EnableTutorialObject();
    }

    void CheckForPlayer()
    {
        playerControls = FindObjectOfType<PlayerControls>();
        if (playerControls == null)
        {
            Invoke("CheckForPlayer",1f);
        }
    }

    void EnableTutorialObject()
    {
        if (tutorialGameObjects[(int)calibrationScreen] == null)
        {
            this.GameControllerAnimator.SetBool("Dark",true);
        }
        else
        {
            this.GameControllerAnimator.SetBool("Dark", false);
        }

        for (int i = 0; i < tutorialGameObjects.Length; i++)
        {
            if (tutorialGameObjects[i] != null)
            {
                tutorialGameObjects[i].SetActive(i == (int)calibrationScreen);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }

        if (calibrationScreen == CalibrationScreen.CalibrateNeutral)
        {
            if (progressBar.progress < progressBar.maxProgress)
            {
                progressBar.progress += Time.deltaTime * 20f;

                for (int i = 1; i <= 12; i++)
                {
                    axisDefaultValue[i] = Input.GetAxis("JoystickAxis" + i);
                }

                for (int i = 0; i < 20; i++) // Assuming maximum 20 buttons per joystick
                {
                    // Check if the current button is pressed
                    if (Input.GetButton("joystick 1 button " + i))
                    {
                        buttonDefaultValue[i] = Input.GetButton("joystick 1 button " + i);
                    }
                }
            }
            else
            {
                if (playerControls != null)
                {
                    CalibrateNeutralPanel.SetActive(false);
                    if (configCompleted)
                    {
                        CalibrationCompletePanel.SetActive(true);
                        calibrationScreen = CalibrationScreen.CompleteScreen;
                        
                        foreach(KeyValuePair<string, string> pair in PlayerControls.controls)
                        {
                            if (PlayerControls.controls[pair.Key].Contains("Axis"))
                            {
                                PlayerControls.neutralValue[pair.Key] = Input.GetAxis(pair.Value);
                            }
                        }

                        Destroy(TopLevelPrefab);
                        //Invoke("Hide", 1f);
                    }
                    else
                    {
                        CalibrateInputPanel.SetActive(true);
                        calibrationScreen++;
                        EnableTutorialObject();
                    }

                }
            }
        }
        else
        {
            if (stepCompleted && !CalibrationCompletePanel.activeSelf)
            {
                if (progressBar.progress < progressBar.maxProgress)
                {
                    progressBar.progress += Time.deltaTime * 150f;
                }
                else
                {
                    Debug.Log("Going to next step");
                    NextStep();
                    //CalibrateNeutralPanel.SetActive(false);
                    //MoveLeftPanel.SetActive(true);
                }
            }
        }

        if (!stepCompleted && !CalibrationCompletePanel.activeSelf)
        {
            string selectedAxis = "";

            for (int i = 0; i < 20; i++) // Assuming maximum 20 buttons per joystick
            {
                // Check if the current button is pressed
                if (Input.GetButton("joystick 1 button " + i) != buttonDefaultValue[i])
                {
                    selectedAxis = "joystick 1 button " + i;
                    SelectInputForAction(
                        requestedInputActionName[(int)calibrationScreen],
                        selectedAxis,
                        false
                        );
                }
            }

            for (int i = 1; i <= 12; i++)
            {
                float axisValue = Input.GetAxis("JoystickAxis" + i);
                if (Mathf.Abs(axisValue - axisDefaultValue[i]) > .85f)
                {
                    //Axis has been moved from neutral
                    selectedAxis = "JoystickAxis" + i;

                    SelectInputForAction(
                        requestedInputActionName[(int)calibrationScreen],
                        selectedAxis,
                        (axisValue < 0),
                        axisDefaultValue[i]
                        );
                }

                //debugText.text += "JoystickAxis" + i + " value: " + axisValue + "\n";
            }
            /*
            if (Input.anyKey)
            {
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(kcode))
                    {
                        //Debug.Log("KeyCode down: " + kcode);
                        selectedAxis = "keycode " + kcode;

                        SelectInputForAction(
                            requestedInputActionName[(int)calibrationScreen],
                            selectedAxis,
                            false,
                            0
                            );
                    }
                }
            }*/

        }
    }

    void SelectInputForAction(string action, string input, bool invert, float neutralValue = 0)
    {
        selectedInputLabel.text = input;
        GreenCheckMark.SetActive(true);
        stepCompleted = true;
        progressBar.progress = 0;
        Debug.Log("invert "+ action + " = " + invert);
        PlayerControls.controls[action] = input;
        PlayerControls.invertControls[action] = invert;
        PlayerControls.neutralValue[action] = neutralValue;
    }

    void NextStep()
    {
        GreenCheckMark.SetActive(false);
        stepCompleted = false;

        if ((int)calibrationScreen < requestedInputLabels.Length - 1)
        {
            calibrationScreen++;
            Debug.Log("NextStep calibrationScreen = " + calibrationScreen);
            requestedInputLabel.text = requestedInputLabels[(int)calibrationScreen];
            progressBar.progress = 0;
            selectedInputLabel.text = "waiting for input";
        }
        else
        {
            CalibrateInputPanel.SetActive(false);
            CalibrateNeutralPanel.SetActive(true);
            GreenCheckMark.SetActive(false);
            configCompleted = true;
            calibrationScreen = CalibrationScreen.CalibrateNeutral;
        }
        EnableTutorialObject();
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}