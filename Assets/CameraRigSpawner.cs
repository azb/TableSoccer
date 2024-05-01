using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigSpawner : MonoBehaviour
{
    public GameObject SpectatorCameraRig2D;
    public GameObject AppleVisionProCameraRig;
    public GameObject MetaQuestCameraRig;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Gets Here Start method of CameraRigSpawner");
#if UNITY_EDITOR
        Debug.Log("Spawning SpectatorCameraRig2D");
        Instantiate(SpectatorCameraRig2D);
#elif UNITY_VISIONOS
        Debug.Log("Spawning AppleVisionProCameraRig");
        Instantiate(AppleVisionProCameraRig);
#elif UNITY_ANDROID
        Debug.Log("Spawning MetaQuestCameraRig");
        Instantiate(MetaQuestCameraRig);
#endif
    }
}
