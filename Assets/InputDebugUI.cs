using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

public class InputDebugUI : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    private void Start()
    {
        string[] joystickNames = Input.GetJoystickNames();

        // Check if any joysticks are connected
        if (joystickNames.Length == 0)
        {
            Debug.Log("No joysticks are connected.");
        }
        else
        {
            // Print the names of all connected joysticks
            for (int i = 0; i < joystickNames.Length; i++)
            {
                Debug.Log("Joystick " + (i + 1) + ": " + joystickNames[i]);
            }
        }
    }

    void Update()
    {
        debugText.text = ""+Time.frameCount+"\n";
        for (int i = 1; i <= 12; i++)
        {
            float axisValue = Input.GetAxis("JoystickAxis" + i);
            debugText.text += "JoystickAxis" + i + " value: " + axisValue + "\n";
        }

        for (int i = 0; i < 12; i++) // Assuming maximum 20 buttons per joystick
        {
            // Check if the current button is pressed
            if (Input.GetButton("joystick 1 button "+i))
            {
                Debug.Log("Joystick button " + (i) + " is pressed!");
            }
        }
    }
}
