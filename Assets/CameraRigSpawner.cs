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
#if UNITY_VISIONOS
        Instantiate(AppleVisionProCameraRig);
#elif UNITY_EDITOR
        Instantiate(SpectatorCameraRig2D);
#elif UNITY_ANDROID
        Instantiate(MetaQuestCameraRig);
#endif
    }
}
