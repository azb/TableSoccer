using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetTeamColors : MonoBehaviour
{
    PlayerControls player;
    MeshRenderer[] meshRenderers;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
            return;

        player = GetComponent<PlayerControls>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i].gameObject.name == "body")
            {
                meshRenderers[i].material = player.teamColorMaterials[(int)player.teamID];
            }
        }
    }
}
