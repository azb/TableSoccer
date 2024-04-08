/*!
* Copyright (C) 2023  Leia, Inc.
*
* This software has been provided under the Leia license agreement.
* You can find the agreement at https://www.leiainc.com/legal/license-agreement
*
* This source code is considered Creator Materials under the definitions of the Leia license agreement.
*/
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net.Http;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;
#if UNITY_2021_1_OR_NEWER
using System.IO.Compression;
#endif

public class MixedRealitySetupWizard : EditorWindow
{

    [MenuItem("MR/Setup")]
    private static void OpenSDKReleaseWizardWindow()
    {
        MixedRealitySetupWizard window = (MixedRealitySetupWizard)EditorWindow.GetWindow(typeof(MixedRealitySetupWizard));
        window.titleContent = new GUIContent("Mixed Reality Setup Wizard");
        window.Show();
    }

    class Task
    {
        public string description;
        public enum State { TODO, DONE };
        public State state;
        public string taskAction;
        public Task(string description, State state, string taskAction)
        {
            this.description = description;
            this.state = state;
            this.taskAction = taskAction;
        }
    }

    bool testTaskIsComplete;


    string GetAssetsFolderPath()
    {
        // Get the path to a script in the "Assets" folder
        MonoScript script = MonoScript.FromScriptableObject(this);
        string scriptPath = AssetDatabase.GetAssetPath(script);

        // Remove the script filename to get the "Assets" folder path
        return Application.dataPath;
    }

    string V3PackagePath
    {
        get
        {
            return Application.dataPath + "/Leia";
        }
    }

    void TestEvent()
    {
        Debug.Log("Executed Test Event");
        testTaskIsComplete = !testTaskIsComplete;
    }


    void OpenNextScene()
    {
        int currentSceneIndex = GetCurrentSceneIndex();

        Debug.Log("Current scene index: " + currentSceneIndex);

        if (currentSceneIndex == -1)
        {
            OpenSceneByName(EditorBuildSettings.scenes[0].path);
        }
        else
        {
            if (currentSceneIndex >= EditorBuildSettings.scenes.Length - 1)
            {
                currentSceneIndex = -1;
            }
            Debug.Log("Attempting to open scene with index: " + currentSceneIndex);
            OpenSceneByName(EditorBuildSettings.scenes[currentSceneIndex + 1].path);

            Debug.Log("Opening scene: " + EditorBuildSettings.scenes[currentSceneIndex + 1].path);
        }
    }
    void AddUpgradeToolPrefab()
    {
        this.InstantiatePrefab("UpgradeTool");
    }

    void AddOVRPassthroughLayerComponent()
    {
        OVRManager ovrManager = FindObjectOfType<OVRManager>();
        OVRPassthroughLayer ovrPassthroughLayer = ovrManager.gameObject.AddComponent<OVRPassthroughLayer>();
        //ovrPassthroughLayer.projectionSurfaceType = OVRPassthroughLayer.ProjectionSurfaceType.Reconstructed;
        //ovrPassthroughLayer.compositionDepth = 1;
        Selection.activeGameObject = ovrManager.gameObject;
    }

    void SetOVRPassthroughLayerComponentPlacement()
    {
        OVRPassthroughLayer ovrPassthroughLayer = FindObjectOfType<OVRPassthroughLayer>();
        ovrPassthroughLayer.overlayType = OVROverlay.OverlayType.Underlay;
        Selection.activeGameObject = ovrPassthroughLayer.gameObject;
    }

    void SetOVRPassthroughLayerComponentDepth()
    {
        OVRPassthroughLayer ovrPassthroughLayer = FindObjectOfType<OVRPassthroughLayer>();
        
        Undo.RecordObject(ovrPassthroughLayer, "Set compositionDepth to 1");

        ovrPassthroughLayer.compositionDepth = 1;
        Selection.activeGameObject = ovrPassthroughLayer.gameObject;
    }

    void SetOVRManagerPassthroughSupport()
    {
        var projectConfig = OVRProjectConfig.CachedProjectConfig;
        projectConfig.insightPassthroughSupport =
            OVRProjectConfig.FeatureSupport.Supported;

        OVRManager ovrManager = FindObjectOfType<OVRManager>();
        Selection.activeGameObject = ovrManager.gameObject;
    }

    void SetOVRManagerPassthroughEnabled()
    {
        OVRManager ovrManager = FindObjectOfType<OVRManager>();
        Undo.RecordObject(ovrManager, "Set Insight Passthrough Enabled");
        ovrManager.isInsightPassthroughEnabled = true;
        Selection.activeGameObject = ovrManager.gameObject;
    }

    void AddOVRPlayerControllerPrefab()
    {
        Debug.LogError("To do this step drag and drop the OVRPlayerController prefab into your scene");
    }

    void SetCenterEyeAnchorCameraBackgroundToTransparent()
    {
        GameObject CenterEyeAnchor = GameObject.Find("CenterEyeAnchor");
        Camera camera = CenterEyeAnchor.GetComponent<Camera>();
        Undo.RecordObject(camera, "Set camera background to transparent");
        camera.backgroundColor = new Color(0, 0, 0, 0);
    }

    void SetSkyboxToNull()
    {
        RenderSettings.skybox = null;
    }


    private void OnGUI()
    {
        List<Task> tasks = new List<Task>();

        string fileContents = "";

        //Debug.Log("fileContents = " + fileContents);

        bool AddedOVRPassthroughLayerComponent = FindObjectOfType<OVRPassthroughLayer>();

        OVRPassthroughLayer ovrPassthroughLayer = FindObjectOfType<OVRPassthroughLayer>();
        OVRManager ovrManager = FindObjectOfType<OVRManager>();
        var projectConfig = OVRProjectConfig.CachedProjectConfig;

        tasks.Add(
            new Task(
            "Add OVRPlayerController prefab",
            ovrManager != null ? Task.State.DONE : Task.State.TODO,
            "AddOVRPlayerControllerPrefab"
            )
        );

        tasks.Add(
            new Task(
            "Set OVRManager Passthrough Enabled",
            ovrManager != null && ovrManager.isInsightPassthroughEnabled ? Task.State.DONE : Task.State.TODO,
            "SetOVRManagerPassthroughEnabled"
            )
        );

        tasks.Add(
                new Task(
                "Set OVRManager Passthrough Support",
                ovrManager != null
                && projectConfig.insightPassthroughSupport == OVRProjectConfig.FeatureSupport.Supported
                ? Task.State.DONE : Task.State.TODO,
                "SetOVRManagerPassthroughSupport"
                )
            );

        tasks.Add(
            new Task(
            "Add OVRPassthroughLayer Component",
            AddedOVRPassthroughLayerComponent ? Task.State.DONE : Task.State.TODO,
            "AddOVRPassthroughLayerComponent"
            )
        );

        tasks.Add(
            new Task(
            "Set OVRPassthroughLayer Placement to Underlay",
            ovrPassthroughLayer != null && ovrPassthroughLayer.overlayType == OVROverlay.OverlayType.Underlay ? Task.State.DONE : Task.State.TODO,
            "SetOVRPassthroughLayerComponentPlacement"
            )
        );

        tasks.Add(
            new Task(
            "Set OVRPassthroughLayer Depth to 1",
            ovrPassthroughLayer != null && ovrPassthroughLayer.compositionDepth == 1 ? Task.State.DONE : Task.State.TODO,
            "SetOVRPassthroughLayerComponentDepth"
            )
        );

        GameObject CenterEyeAnchor = GameObject.Find("CenterEyeAnchor");
        Camera centerEyeCamera = null;

        if (CenterEyeAnchor != null)
        {
            centerEyeCamera = CenterEyeAnchor.GetComponent<Camera>();
        }

        tasks.Add(
            new Task(
            "Set CenterEyeAnchor Camera Background to black and fully transparent\n(RGBA 0,0,0,0)",
            CenterEyeAnchor != null
            && centerEyeCamera != null
            && centerEyeCamera.backgroundColor == new Color(0, 0, 0, 0) ? Task.State.DONE : Task.State.TODO,
            "SetCenterEyeAnchorCameraBackgroundToTransparent"
            )
        );

        Material currentSkybox = RenderSettings.skybox;

        tasks.Add(
            new Task(
            "Set skybox to null",
            currentSkybox == null ? Task.State.DONE : Task.State.TODO,
            "SetSkyboxToNull"
            )
        );

        bool SceneHasMainCamera = GameObject.FindWithTag("MainCamera");
        if (!SceneHasMainCamera && CenterEyeAnchor != null)
        {
            string fileName = this.GetAssetsFolderPath() + "/Oculus/VR/Scripts/OVROverlay.cs";
            string fileText = File.ReadAllText(fileName);
            if (CenterEyeAnchor.tag != "" && fileText.Contains(CenterEyeAnchor.tag))
            {
                SceneHasMainCamera = true;
            }
        }
        
        tasks.Add(
            new Task(
            "Ensure main camera is tagged as MainCamera or change the tag name in\nOVROverlay.cs your VR Camera's tag",
            SceneHasMainCamera ? Task.State.DONE : Task.State.TODO,
            "SetSkyboxToNull"
            )
        );


        List<string> scenes = new List<string>();
        int currentSceneIndex = GetCurrentSceneIndex();

        if (currentSceneIndex >= 0 && currentSceneIndex < EditorBuildSettings.scenes.Length && EditorBuildSettings.scenes[currentSceneIndex].enabled)
        {
            scenes.Add(EditorBuildSettings.scenes[currentSceneIndex].path);
        }



        // Create a GUIStyle
        GUIStyle incompleteStyle = new GUIStyle(GUI.skin.label);
        GUIStyle completeStyle = new GUIStyle(GUI.skin.label);

        // Set the text color of the GUIStyle
        incompleteStyle.normal.textColor = Color.red;
        completeStyle.normal.textColor = Color.green;

        GUILayout.Space(10f);
        GUILayout.Label("This tool will help you setup Mixed Reality in your project");
        GUILayout.Space(20f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Task", GUILayout.Width(450f));
        GUILayout.Label("Status");
        GUILayout.EndHorizontal();
        GUILayout.Space(20f);

        foreach (Task task in tasks)
        {
            GUIStyle style = incompleteStyle;

            if (task.state == Task.State.DONE)
            {
                style = completeStyle;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label(task.description, GUILayout.Width(450f));
            GUILayout.Label(task.state.ToString(), style);

            GUILayout.EndHorizontal();
            GUILayout.Space(10f);

            if (task.state != Task.State.DONE && GUILayout.Button("DO IT", GUILayout.Width(200), GUILayout.Height(30)))
            {
                System.Reflection.MethodInfo methodInfo = GetType().GetMethod(task.taskAction,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (methodInfo == null)
                {
                    Debug.LogError("methodInfo is null");
                }
                methodInfo.Invoke(this, null);
            }
            GUILayout.Space(20f);
        }

        GUILayout.Space(20f);

        GUILayout.FlexibleSpace(); // Add flexible space to push buttons to the bottom

        GUILayout.BeginHorizontal();

        GUI.enabled = true;

        GUILayout.EndHorizontal();
    }

    private void OpenGitHubDesktop()
    {
        string repoUrl = "https://github.com/LeiaInc/LeiaLoftUnityPackage";
        System.Diagnostics.Process.Start("github-windows://openRepo/" + repoUrl);
    }


    private void CopyFolder(string sourceFolderPath, string destinationFolderPath)
    {
        DirectoryInfo sourceDirectory = new DirectoryInfo(sourceFolderPath);
        DirectoryInfo destinationDirectory = new DirectoryInfo(destinationFolderPath);

        CopyAllFiles(sourceDirectory, destinationDirectory);

        // Recursively copy subdirectories
        foreach (DirectoryInfo sourceSubdirectory in sourceDirectory.GetDirectories())
        {
            DirectoryInfo destinationSubdirectory = destinationDirectory.CreateSubdirectory(sourceSubdirectory.Name);
            CopyAllFiles(sourceSubdirectory, destinationSubdirectory);
        }
    }

    private void CopyAllFiles(DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory)
    {
        foreach (FileInfo file in sourceDirectory.GetFiles())
        {
            string destinationFilePath = Path.Combine(destinationDirectory.FullName, file.Name);
            file.CopyTo(destinationFilePath, true);
        }
    }

    private void CopyAndRenameFile(string fileToCopy, string newName)
    {
        // Get the full paths of source and destination files
        string sourceFullPath = Path.Combine(Application.streamingAssetsPath, fileToCopy);
        string destinationFullPath = Path.Combine(Application.streamingAssetsPath, newName);

        // Copy and rename the file
        if (File.Exists(sourceFullPath))
        {
            File.Copy(sourceFullPath, destinationFullPath, true);
            Debug.Log("File copied and renamed successfully.");
        }
        else
        {
            Debug.LogError("Source file not found.");
        }
    }

    private void ZipFolderAtPath(string sourceFolderPath, string outputZipPath)
    {
        Debug.Log("Attempting to zip " + sourceFolderPath + " to " + outputZipPath);
        if (Directory.Exists(sourceFolderPath))
        {
            try
            {
#if UNITY_2021_1_OR_NEWER
                ZipFile.CreateFromDirectory(sourceFolderPath, outputZipPath);
                Debug.Log("Folder zipped successfully.");
#else
                Debug.LogError("Zipping not supported in Unity 2020 and earlier. Try using Unity 2021 or later.");
#endif
            }
            catch (IOException e)
            {
                Debug.LogError("IO error zipping folder: " + e.Message);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error zipping folder: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Source folder not found.");
        }
    }

    static async void DownloadFileAsync(string fileUrl, string savePath)
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.GetAsync(fileUrl))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                        stream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await contentStream.CopyToAsync(stream);

                        Debug.Log("Finished downloading Leia.zip! Extracting");
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
    }

    private void OpenSceneByName(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(sceneName);
        }
    }

    public static int GetCurrentSceneIndex()
    {
        // Get the currently active scene
        Scene activeScene = EditorSceneManager.GetActiveScene();

        // Iterate through all scenes and find the index of the active scene
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
            if (scene.path == activeScene.path)
            {
                return i;
            }
        }

        // If the active scene is not found, return -1 or handle it as needed
        return -1;
    }

    private void InstantiatePrefab(string prefabName)
    {
        GameObject prefab = Resources.Load(prefabName) as GameObject;

        if (prefab != null)
        {
            GameObject instantiatedObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            if (instantiatedObject != null)
            {
                instantiatedObject.transform.SetAsLastSibling();
                Selection.activeGameObject = instantiatedObject;
                // Optionally, you can perform additional setup for the instantiated object
                Debug.Log("Prefab instantiated: " + instantiatedObject.name);
            }
            else
            {
                Debug.LogError("Failed to instantiate the prefab.");
            }
        }
        else
        {
            Debug.LogError("Prefab with name '" + prefabName + "' not found in Resources folder.");
        }
    }

    private static void AddCompileSymbol(string symbolToAdd)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(symbolToAdd))
        {
            Debug.LogError("Symbol cannot be empty or null.");
            return;
        }

        // Get the current scripting define symbols for the selected build target
        string buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup.ToString();
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

        // Check if the symbol already exists
        if (!currentSymbols.Contains(symbolToAdd))
        {
            // Add the new symbol
            string newSymbols = currentSymbols + ";" + symbolToAdd;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newSymbols);

            // Log to console for confirmation
            Debug.Log(symbolToAdd + " added to scripting define symbols for " + buildTargetGroup);
        }
        else
        {
            Debug.LogWarning(symbolToAdd + " already exists in scripting define symbols for " + buildTargetGroup);
        }
#endif
    }

    private static void RemoveCompileSymbol(string symbolToRemove)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(symbolToRemove))
        {
            Debug.LogError("Symbol cannot be empty or null.");
            return;
        }

        // Get the current scripting define symbols for the selected build target
        string buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup.ToString();
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

        // Check if the symbol exists
        if (currentSymbols.Contains(symbolToRemove))
        {
            // Remove the symbol
            string[] symbols = currentSymbols.Split(';');
            string newSymbols = string.Join(";", symbols.Where(s => s != symbolToRemove));

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newSymbols);

            // Log to console for confirmation
            Debug.Log(symbolToRemove + " removed from scripting define symbols for " + buildTargetGroup);
        }
        else
        {
            Debug.LogWarning(symbolToRemove + " does not exist in scripting define symbols for " + buildTargetGroup);
        }
#endif
    }
}
#endif
