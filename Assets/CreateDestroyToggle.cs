
using UnityEngine;

public class CreateDestroyToggle : MonoBehaviour
{
    public GameObject PrefabToToggle;
    public GameObject CreatedGameObject;

    public void Toggle()
    {
        Debug.Log("Toggle");
        
        if (CreatedGameObject != null)
        {
            Debug.Log("Destroying");
            //CreatedGameObject.SetActive(false);
            Destroy(CreatedGameObject);
            CreatedGameObject = null;
        }
        else
        {
            Debug.Log("Creating");
            CreatedGameObject = Instantiate(PrefabToToggle);
        }
    }
}
