
using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

public class RuntimeNavMeshBuilder : MonoBehaviour
{
    private NavMeshSurface surface;

    public TemplateRoomFiller templateRoomFiller;

    public bool NavMeshReady { get; private set; }

    private IEnumerator Start()
    {
        Debug.Log("NavMesh builder started.");

        surface = GetComponent<NavMeshSurface>();

        if (surface == null)
        {
            Debug.LogError("No NavMeshSurface found!");
            yield break;
        }

        // Wait until all warehouse rooms are generated.
        while (!templateRoomFiller.GenerationComplete)
        {
            yield return null;
        }

        // Wait one extra frame for destroyed placeholders.
        yield return null;

        Debug.Log("Starting NavMesh build...");

        surface.BuildNavMesh();

        NavMeshReady = true;

        Debug.Log("Runtime NavMesh build finished.");
        Debug.Log("NavMesh Data Exists: " +
            (surface.navMeshData != null));
    }
}
