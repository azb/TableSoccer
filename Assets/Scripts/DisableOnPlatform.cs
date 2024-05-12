using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnPlatform : MonoBehaviour
{
    public bool MetaQuest;
    public bool VisionOS;

    // Start is called before the first frame update
    void Awake()
    {
        if (MetaQuest && Application.platform == RuntimePlatform.Android)
        {
            gameObject.SetActive(false);
        }
        if (VisionOS && Application.platform == RuntimePlatform.VisionOS)
        {
            gameObject.SetActive(false);
        }
    }
}
