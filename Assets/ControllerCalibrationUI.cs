using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControllerCalibrationUI : MonoBehaviour
{
    public TextMeshProUGUI selectedInputLabel;
    public TextMeshProUGUI requestedInputLabel;

    string[] requestedInputLabels =
    {
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
        "Neutral",
        "MoveX",
        "MoveY",
        "AimX",
        "AimY",
        "Kick"
    };


    public ProgressBar progressBar;
    public GameObject GreenCheckMark;


    public GameObject CalibrateNeutralPanel;

    public GameObject CalibrateInputPanel;

    public GameObject CalibrationCompletePanel;

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

    CalibrationScreen calibrationScreen = CalibrationScreen.CalibrateNeutral;

    public enum CalibrationScreen {
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

    float[] axisDefaultValue = new float[13];
    bool[] buttonDefaultValue = new bool[20];

    bool stepCompleted = false;

    PlayerControls playerControls;

    // Start is called before the first frame update
    void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
        GreenCheckMark.SetActive(false);

        for (int i = 1; i <= 12; i++)
        {
            axisDefaultValue[i] = Input.GetAxis("JoystickAxis" + i);
        }

        for (int i = 0; i < 20; i++)
        {
            buttonDefaultValue[i] = Input.GetButton("joystick 1 button " + i);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                calibrationScreen++;
                CalibrateNeutralPanel.SetActive(false);
                CalibrateInputPanel.SetActive(true);
            }
        }
        else
        {
            if (stepCompleted && !CalibrationCompletePanel.activeSelf)
            {
                if (progressBar.progress < progressBar.maxProgress)
                {
                    progressBar.progress += Time.deltaTime * 75f;
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

            for (int i = 1; i <= 12; i++)
            {
                float axisValue = Input.GetAxis("JoystickAxis" + i);
                if (Mathf.Abs(axisValue - axisDefaultValue[i]) > .85f)
                {
                    //Axis has been moved from neutral
                    selectedAxis = "JoystickAxis" + i;
                    /*
                    if (axisValue > 0)
                    {
                        selectedAxis += "+";
                    }
                    else
                    {
                        selectedAxis += "-";
                    }*/

                    SelectInputForAction(
                        requestedInputActionName[(int)calibrationScreen],
                        selectedAxis,
                        (axisValue < 0)
                        );
                }

                //debugText.text += "JoystickAxis" + i + " value: " + axisValue + "\n";
            }

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
        }
    }

    void SelectInputForAction(string action, string input, bool invert)
    {
        selectedInputLabel.text = input;
        GreenCheckMark.SetActive(true);
        stepCompleted = true;
        progressBar.progress = 0;
        playerControls.controls[action] = input;
        playerControls.invertControls[action] = invert;
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
            CalibrationCompletePanel.SetActive(true);
            GreenCheckMark.SetActive(false);
            Invoke("Hide", 5f);
        }
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}