#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CompileSymbols : MonoBehaviour
{
    public static void Add(string defineSymbol)
    {
        // Get the current target group
        BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        // Get the current scripting define symbols for this target group
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        // Check if the define already exists
        if (!defines.Contains(defineSymbol))
        {
            // If not, add the new define
            if (string.IsNullOrEmpty(defines))
            {
                defines = defineSymbol;
            }
            else
            {
                defines += ";" + defineSymbol;
            }

            // Set the updated scripting define symbols for this target group
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);

            Debug.Log("Added define: " + defineSymbol);
        }
        else
        {
            Debug.LogWarning("Define already exists: " + defineSymbol);
        }
    }

    public static void Remove(string defineSymbol)
    {
        // Get the current target group
        BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        // Get the current scripting define symbols for this target group
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        // Check if the define exists
        if (defines.Contains(defineSymbol))
        {
            // Remove the define
            string[] allDefines = defines.Split(';');
            defines = string.Join(";", allDefines.Where(d => d != defineSymbol).ToArray());

            // Set the updated scripting define symbols for this target group
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);

            Debug.Log("Removed define: " + defineSymbol);
        }
        else
        {
            Debug.LogWarning("Define not found: " + defineSymbol);
        }
    }
}
#endif