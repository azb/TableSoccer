using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-10000)]
public class NetworkSync : MonoBehaviourPunCallbacks
{
    int viewIDsIssued;
    Dictionary<int, NetworkedObject> instantiatedNetworkedObjects = new Dictionary<int, NetworkedObject>();

    public static NetworkSync _instance;
    public static NetworkSync Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkSync>();
            }
            if (_instance == null)
            {
                Debug.LogError("No NetworkSync object found in the scene!");
            }

            return _instance;
        }
    }
    public NetworkedObject GameBoardNetworkedObject;

    PhotonView _photonView;

    public Transform PlayerPrefab;
    public Transform[] SpawnPoints;
    public Material[] PlayerColorMaterials;

    public PhotonView photonView
    {
        get
        {
            if (_photonView == null)
            {
                _photonView = Instance.GetComponent<PhotonView>();
            }
            return _photonView;
        }
    }

    public static Material GetPlayerMaterial(int PlayerID)
    {
        int numberOfColors = Instance.PlayerColorMaterials.Length;
        return Instance.PlayerColorMaterials[
            Mathf.Abs(PlayerID % numberOfColors)
        ];
    }


    // Function to get the actor number by UserId
    public static int GetActorNumberByUserId(string userId)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log("PlayerList player.UserId: " + player.UserId);
            if (player.UserId == userId)
            {
                return player.ActorNumber;
            }
        }
        return -1; // Return -1 if UserId is not found in the room
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        //create all existing objects on the new player's side
        if (IsMasterClient)
        {
            Debug.Log("IsMasterClient so sending objects: " + instantiatedNetworkedObjects.Count);
            foreach (KeyValuePair<int, NetworkedObject> keyValuePair in instantiatedNetworkedObjects)
            {
                NetworkedObject networkedObject = keyValuePair.Value;
                if (networkedObject == null)
                {
                    continue;
                }
                Debug.Log("Requesting to Instantiate prefab " + networkedObject.PrefabName + " for object : " + networkedObject.transform.name);
                int id = keyValuePair.Key;

                int ParentID = -1;
                if (networkedObject.transform.parent != null)
                {
                    NetworkedObject parentNetworkedObject = networkedObject.transform.parent.GetComponent<NetworkedObject>();
                    if (parentNetworkedObject != null)
                    {
                        ParentID = parentNetworkedObject.ID;
                    }
                }

                Instance.photonView.RPC(
                    "InstantiateWithNetworkedObjectRPC",
                    RpcTarget.Others,
                    networkedObject.PrefabName,
                    networkedObject.transform.position,
                    networkedObject.transform.rotation,
                    ParentID,
                    networkedObject.ID,
                    networkedObject.OwnerID
                );
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (PlayerPrefab != null)
        {
            Transform newPlayer = Create(
                PlayerPrefab.name,
                SpawnPoints[PlayerCount - 1].localPosition,
                SpawnPoints[PlayerCount - 1].rotation,
                GameBoardNetworkedObject,
                PhotonNetwork.LocalPlayer.ActorNumber
            );

            NetworkedPlayer newNetworkedPlayer = newPlayer.GetComponent<NetworkedPlayer>();
            //newNetworkedPlayer.PlayerID = PhotonNetwork.LocalPlayer.ActorNumber;

            newPlayer.GetComponent<NetworkedObject>().SetSyncedInt(
                "PlayerID",
                newNetworkedPlayer.PlayerID
            );

            LocalPlayerID = newNetworkedPlayer.PlayerID;
        }
    }

    public UnityEvent OnDisconnectedAction;

    public DisconnectCause disconnectCause;

    public override void OnDisconnected(DisconnectCause cause)
    {
        // This method will be called when the connection is lost for any reason.
        Debug.Log("Connection lost. Reason: " + cause);
        // You can perform actions or execute code when the connection is lost here.
        PlayingOnline = false;
        disconnectCause = cause;
        OnDisconnectedAction.Invoke();
    }

    //Playercount returns the number of online players currently connected.
    //If a player disconnects after the game has started, this number will update to reflect that
    //This number does not include AI players
    public static int PlayerCount
    {
        get
        {
            if (PlayingOnline)
            {
                return PhotonNetwork.PlayerList.Length;
            }
            else
            {
                return 1;
            }
        }
    }

    void Awake()
    {
        NetworkedObject[] initialNetworkedObjects = FindObjectsOfType<NetworkedObject>();

        for (int i = 0; i < initialNetworkedObjects.Length; i++)
        {
            if (initialNetworkedObjects[i].ID == -1)
            {
                initialNetworkedObjects[i].ID = GetNextAvailableNetworkedObjectID();

                Instance.instantiatedNetworkedObjects.Add(
                    initialNetworkedObjects[i].ID,
                    initialNetworkedObjects[i]
                );
            }
        }

        _photonView = GetComponent<PhotonView>();
        if (_photonView == null)
        {
            Debug.LogError("NetworkSync game object must have a PhotonView attached to it!");
        }
    }

    public static NetworkedObject GetNetworkedObjectWithID(int id)
    {
        NetworkedObject networkedObject;
        Instance.instantiatedNetworkedObjects.TryGetValue(id, out networkedObject);
        if (networkedObject == null)
        {
            //Debug.LogErrorFormat("NetworkedObject with id {0} not instantiated ", id);
        }
        return networkedObject;
    }
    //public static bool PlayingOnline
    //{
    //    get
    //    { 
    //    return !PhotonNetwork.OfflineMode;
    //    }
    //}

    public static bool PlayingOnline
    {
        get
        {
            return (PlayerPrefs.GetInt("playingOnline") == 1);
        }
        set
        {
            PlayerPrefs.SetInt("playingOnline", Convert.ToInt32(value));
        }
    }


    public static bool PlayingOffline
    {
        get
        {
            return !PlayingOnline;
        }
    }

    public static int LocalPlayerID;

    //NetworkSync.IsMasterClient
    //Returns true if playing offline or if is the master client in online game
    public static bool IsMasterClient
    {
        get
        {
            bool isMaster =
                PhotonNetwork.IsMasterClient;
            //|| !PlayingOnline;

            //Debug.Log("IsMasterClient = "+isMaster);

            return isMaster;
        }
    }

    public static Transform Create(
        string PrefabName,
        Vector3 LocalPosition,
        Quaternion Rotation,
        NetworkedObject Parent,
        int OwnerID = -1 //Use this argument to specify which player owns / controls this object if this is a controllable object like a player character
        )
    {
        Transform ParentTransform = null;
        if (Parent != null)
        {
            ParentTransform = Parent.transform;
        }
        //Instantiate with normal instantiate (not RPC) so that we can return the Transform from this method
        Transform newTransform = Instantiate(
            Resources.Load<Transform>(PrefabName),
            LocalPosition,
            Rotation,
            ParentTransform
            ).transform;

        newTransform.localPosition = LocalPosition;

        NetworkedObject networkedObject = newTransform.gameObject.GetComponent<NetworkedObject>();
        if (networkedObject == null)
        {
            networkedObject = newTransform.gameObject.AddComponent<NetworkedObject>();
        }
        networkedObject.PrefabName = PrefabName;
        networkedObject.OwnerID = OwnerID;

        if (networkedObject == null)
        {
            Debug.LogError(newTransform.name + "doesn't have a NetworkedObject component attached to it, attaching one.");
            newTransform.gameObject.AddComponent<NetworkedObject>();
        }
        int newViewID = Instance.GetNextAvailableNetworkedObjectID();

        networkedObject.ID = newViewID;

        Debug.Log("Adding instantiatedNetworkedObject: " + networkedObject.transform.name);
        Instance.instantiatedNetworkedObjects.Add(networkedObject.ID, networkedObject);

        if (PlayingOnline)
        {
            Instance.photonView.RPC(
                "InstantiateWithNetworkedObjectRPC",
                RpcTarget.Others,
                PrefabName,
                LocalPosition,
                Rotation,
                Parent.ID,
                newViewID,
                OwnerID
            );
        }

        return newTransform;
    }

    //internal method used by the Create method to generate new NetworkedObjects
    [PunRPC]
    void InstantiateWithNetworkedObjectRPC(
           string PrefabName,
           Vector3 localPosition,
           Quaternion rotation,
           int ParentNetworkedObjectID,
           int ID,
           int OwnerID
           )
    {
        if (instantiatedNetworkedObjects.ContainsKey(ID))
        {
            //This object has already been created on this client, so skip creating it again
            return;
        }

        NetworkedObject parentNetworkedObject = NetworkSync.GetNetworkedObjectWithID(ParentNetworkedObjectID);

        Transform parent = null;

        if (parentNetworkedObject != null)
        {
            parent = parentNetworkedObject.transform;
            Debug.Log("Creating a " + PrefabName + " and setting its parent to " + parent.name);
        }
        else
        {
            Debug.Log("Creating a " + PrefabName + " without a parent!");
        }

        Transform newTransform = Instantiate(
            Resources.Load<Transform>(PrefabName),
            localPosition,
            rotation,
            parent
            ).transform;

        newTransform.localPosition = localPosition;

        NetworkedObject networkedObject = newTransform.GetComponent<NetworkedObject>();

        if (networkedObject == null)
        {
            Debug.Log(newTransform.name + "doesn't have a NetworkedObject component attached to it, attaching one.");
            networkedObject = newTransform.gameObject.AddComponent<NetworkedObject>();
        }

        networkedObject.OwnerID = OwnerID;

        networkedObject.PrefabName = PrefabName;

        if (networkedObject == null)
        {
            Debug.LogError("networkedObject is null for game object " + newTransform.name);
        }

        networkedObject.ID = ID;
        Debug.Log("Adding instantiatedNetworkedObject: " + networkedObject.transform.name + " with ID: " + ID);
        instantiatedNetworkedObjects.Add(ID, networkedObject);
    }

    //internal method used by the Create method to generate new NetworkedObjects
    int GetNextAvailableNetworkedObjectID()
    {
        viewIDsIssued++;
        int newViewID = PhotonNetwork.LocalPlayer.ActorNumber * 10000000 + viewIDsIssued;
        //        Debug.LogFormat("Issuing view id {0}", newViewID);
        return newViewID;
    }

    [PunRPC]
    void SetSyncedVariableRPC(int networkObjectID, string variableName, string variableValue)
    {
        NetworkedObject networkedObject = GetNetworkedObjectWithID(networkObjectID);
        if (networkedObject != null)
        {
            networkedObject.SyncedVariables[variableName] = variableValue;
        }
    }

    public Dictionary<GameObject, NetworkedObject> GameObjectToNetworkedObjectMap;

    //A static method for getting the NetworkedObject component of any gameobject
    public static NetworkedObject GetNetworkedObject(GameObject gameObject)
    {
        if (Instance.GameObjectToNetworkedObjectMap.ContainsKey(gameObject))
        {
            return Instance.GameObjectToNetworkedObjectMap[gameObject];
        }
        NetworkedObject result = gameObject.GetComponent<NetworkedObject>();

        Instance.GameObjectToNetworkedObjectMap.Add(
            gameObject,
            result
        );

        return result;
    }

    public static Vector3 PositionOnGameBoard(Vector3 worldPosition)
    {
        return Instance.GameBoardNetworkedObject.transform.InverseTransformPoint(
            worldPosition
        );
    }

    public static Vector3 GameBoardToWorldPosition(Vector3 gameBoardPosition)
    {
        return Instance.GameBoardNetworkedObject.transform.TransformPoint(
            gameBoardPosition
        );
    }

    public static void RestartScene()
    {
        if (PlayingOnline)
        {
            Instance.photonView.RPC(
                "RestartSceneRPC",
                RpcTarget.All
            );
        }
        else
        {
            Instance.RestartSceneRPC();
        }
    }
    
    public void ClearScene()
    {
        //todo: implement
    }

    [PunRPC]
    public void RestartSceneRPC()
    {
        ClearScene();
    }
}
