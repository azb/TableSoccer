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
        "Move Right",
        "Move Forward",
        "Move Backward",
        "Aim Left",
        "Aim Right",
        "Aim Forward",
        "Aim Backward",
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

    bool stepCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        GreenCheckMark.SetActive(false);
        for (int i = 1; i <= 12; i++)
        {
            axisDefaultValue[i] = Input.GetAxis("JoystickAxis" + i);
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
                    if (axisValue > 0)
                    {
                        selectedAxis += "+";
                    }
                    else
                    {
                        selectedAxis += "-";
                    }

                    selectedInputLabel.text = selectedAxis;
                    GreenCheckMark.SetActive(true);
                    stepCompleted = true;
                    progressBar.progress = 0;
                }

                //debugText.text += "JoystickAxis" + i + " value: " + axisValue + "\n";
            }
        }
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
        }
    }

}

