using UnityEngine;
using UnityEditor;
using System.IO;

public class PlatformSwitcherWindow : EditorWindow
{
    private bool isVisionOS = false;
    private bool isMetaQuest = false;

    [MenuItem("Tools/Platform Switcher")]
    public static void ShowWindow()
    {
        GetWindow<PlatformSwitcherWindow>("Platform Switcher");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Target Platform", EditorStyles.boldLabel);

        isVisionOS = EditorGUILayout.Toggle("VisionOS", isVisionOS);
        isMetaQuest = EditorGUILayout.Toggle("Meta Quest", isMetaQuest);

        GUILayout.Space(20);

        if (GUILayout.Button("Switch Platform"))
        {
            if (isVisionOS && isMetaQuest)
            {
                Debug.LogError("Please select only one platform.");
                return;
            }

            if (!isVisionOS && !isMetaQuest)
            {
                Debug.LogError("Please select a platform.");
                return;
            }

            if (isVisionOS)
            {
                Debug.Log("Switching to VisionOS platform...");
                SwitchPlatform("manifest_visionos.json");
            }
            else if (isMetaQuest)
            {
                Debug.Log("Switching to Meta Quest platform...");
                SwitchPlatform("manifest_metaquest.json");
            }
        }
    }

    private void SwitchPlatform(string manifestFileName)
    {
        string sourcePath = Path.Combine(Application.dataPath, "Packages", manifestFileName);
        string destinationPath = Path.Combine(Application.dataPath, "Packages", "manifest.json");

        Debug.Log($"Copying {manifestFileName} to manifest.json...");

        if (File.Exists(destinationPath))
        {
            Debug.Log("Backup existing manifest.json...");
            FileUtil.ReplaceFile(destinationPath, destinationPath + ".backup");
        }
        else
        {
            Debug.Log("No existing manifest.json found.");
        }

        FileUtil.ReplaceFile(sourcePath, destinationPath);

        Debug.Log("Platform switched successfully.");
    }
}
