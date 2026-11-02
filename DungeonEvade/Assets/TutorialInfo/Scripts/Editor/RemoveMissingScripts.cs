using UnityEngine;
using UnityEditor;

public class RemoveMissingScripts
{
    [MenuItem("Tools/Remove Missing Scripts In Scene")]
    static void Remove()
    {
        foreach (GameObject go in Object.FindObjectsOfType<GameObject>(true))
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        }

        Debug.Log("Removed missing scripts.");
    }
}
