using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedObject : MonoBehaviour
{
    public string PrefabName;
    public string _ownerID = "";
    public int OwnerID
    {
        get
        {
            return GetSyncedInt("OwnerID");
        }
        set
        {
            SetSyncedInt("OwnerID", value);
        }
    }

    public bool IsMine
    {
        get
        {
            return NetworkSync.LocalPlayerID == OwnerID;
        }
    }

    private int _ID = -1;
    public int ID
    {
        get
        {
            return _ID;
        }
        set
        {
            _ID = value;
            gameObject.name += "" + _ID;
        }
    }

    public Dictionary<string, string> _syncedVariables;
    public Dictionary<string, string> SyncedVariables
    {
        get
        {
            if (_syncedVariables == null)
            {
                _syncedVariables = new Dictionary<string, string>();
            }
            return _syncedVariables;
        }
        set
        {
            if (_syncedVariables == null)
            {
                _syncedVariables = new Dictionary<string, string>();
            }
            _syncedVariables = value;
        }
    }

    public string GetSyncedString(string VariableName)
    {
        if (!SyncedVariables.ContainsKey(VariableName))
        {
            return "";
        }
        return SyncedVariables[VariableName];
    }
    public Vector3 GetSyncedVector3(string VariableName)
    {
        if (!SyncedVariables.ContainsKey(VariableName))
        {
            //Debug.Log("Key not found: "+ VariableName);
            return Vector3.zero;
        }
        return ParseStringToVector3(SyncedVariables[VariableName]);
    }

    public int GetSyncedInt(string VariableName)
    {
        if (!SyncedVariables.ContainsKey(VariableName))
        {
            //Debug.Log("Key not found: "+ VariableName);
            return 0;
        }
        return int.Parse(SyncedVariables[VariableName]);
    }


    public float GetSyncedFloat(string VariableName)
    {
        if (!SyncedVariables.ContainsKey(VariableName))
        {
            //Debug.Log("Key not found: "+ VariableName);
            return 0;
        }
        return float.Parse(SyncedVariables[VariableName]);
    }

    public bool GetSyncedBool(string VariableName)
    {
        if (!SyncedVariables.ContainsKey(VariableName))
        {
            //Debug.Log("Key not found: "+ VariableName);
            return false;
        }
        return bool.Parse(SyncedVariables[VariableName]);
    }

    public void SetSyncedString(string VariableName, string Value)
    {
        SyncedVariables[VariableName] = Value;

        NetworkSync.Instance.photonView.RPC(
            "SetSyncedVariableRPC",
            RpcTarget.Others,
            ID,
            VariableName,
            Value
            );
    }

    public void SetSyncedVector3(string VariableName, Vector3 Value)
    {
        //Debug.Log("SetSyncedVector3 Adding key: " + VariableName+" value: "+ Value.ToString());

        if (SyncedVariables == null)
        {
            Debug.LogError("Synced variables not created yet");
        }
        SyncedVariables[VariableName] = string.Format("{0},{1},{2}", Value.x, Value.y, Value.z);


        if (NetworkSync.Instance == null)
        {
            Debug.LogError("Synced variables not created yet");
        }

        if (NetworkSync.PlayingOnline)
        {
            NetworkSync.Instance.photonView.RPC(
            "SetSyncedVariableRPC",
            RpcTarget.Others,
            ID,
            VariableName,
            SyncedVariables[VariableName]
            );
        }
    }

    public void SetSyncedInt(string VariableName, int Value)
    {
        SyncedVariables[VariableName] = string.Format("{0}", Value);

        if (NetworkSync.PlayingOnline)
        {
            NetworkSync.Instance.photonView.RPC(
                "SetSyncedVariableRPC",
                RpcTarget.Others,
                ID,
                VariableName,
                SyncedVariables[VariableName]
                );
        }
    }

    public void SetSyncedFloat(string VariableName, float Value)
    {
        //Debug.Log("SetSyncedVector3 Adding key: " + VariableName+" value: "+ Value.ToString());
        SyncedVariables[VariableName] = string.Format("{0}", Value);

        if (NetworkSync.PlayingOnline)
        {
        NetworkSync.Instance.photonView.RPC(
            "SetSyncedVariableRPC",
            RpcTarget.Others,
            ID,
            VariableName,
            SyncedVariables[VariableName]
            );
        }
    }

    public void SetSyncedBool(string VariableName, bool Value)
    {
        //Debug.Log("SetSyncedVector3 Adding key: " + VariableName+" value: "+ Value.ToString());
        SyncedVariables[VariableName] = string.Format("{0}", Value);
        if (NetworkSync.PlayingOnline)
        {
            NetworkSync.Instance.photonView.RPC(
            "SetSyncedVariableRPC",
            RpcTarget.Others,
            ID,
            VariableName,
            SyncedVariables[VariableName]
            );
        }
    }

    public Vector3 ParseStringToVector3(string input)
    {
        Vector3 result = Vector3.zero;

        // Split the input string by commas
        string[] values = input.Split(',');

        if (values.Length == 3)
        {
            // Attempt to parse each value as a float
            float x, y, z;
            if (float.TryParse(values[0], out x) && float.TryParse(values[1], out y) && float.TryParse(values[2], out z))
            {
                result = new Vector3(x, y, z);
            }
            else
            {
                Debug.LogWarning("One or more of the input values is not a valid number.");
            }
        }
        else
        {
            Debug.LogWarning("Input string does not contain three comma-separated values.");
        }

        return result;
    }

    public void Awake()
    {
        Invoke("CheckIfInstantiatedProperly", .1f);
    }

    public bool ShowSyncedVariables;
    public string SyncedVariablesString;

    private void Update()
    {
        if (ShowSyncedVariables)
        {
            SyncedVariablesString = "";
            foreach (var entry in SyncedVariables)
            {
                SyncedVariablesString += $"Key: {entry.Key}, Value: {entry.Value}\n";
            }
        }
    }

    public void CheckIfInstantiatedProperly()
    {
        if (ID == -1)
        {
            Debug.Log("NetworkView Game object not instantiated properly! Use NetworkSync.Create "
                + gameObject.name, gameObject);
        }
    }
}
