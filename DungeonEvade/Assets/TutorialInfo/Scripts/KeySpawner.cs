using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [Header("Key Settings")]
    public GameObject keyPrefab;
    public int keysToSpawn = 5;

    private IEnumerator Start()
    {
        TemplateRoomFiller roomFiller =
            FindObjectOfType<TemplateRoomFiller>();

        if (roomFiller == null)
        {
            Debug.LogError("TemplateRoomFiller not found!");
            yield break;
        }

        while (!roomFiller.GenerationComplete)
        {
            yield return null;
        }

        // Wait one extra frame for generated rooms.
        yield return null;


        // ==========================================
        // TEST MODE: SPAWN ALL KEYS IN EXIT ROOM
        // ==========================================

        /*Transform testSpawnFolder = null;

        Transform[] allObjects =
            FindObjectsOfType<Transform>();

        foreach (Transform obj in allObjects)
        {
            if (obj.name == "TestSpawns")
            {
                testSpawnFolder = obj;
                break;
            }
        }

        if (testSpawnFolder == null)
        {
            Debug.LogError("TestSpawns folder not found!");
            yield break;
        }

        int spawnedKeys = 0;

        foreach (Transform spawnPoint in testSpawnFolder)
        {
            if (spawnedKeys >= keysToSpawn)
                break;

            Instantiate(
                keyPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            Debug.Log("TEST KEY SPAWNED AT: " + spawnPoint.name);

            spawnedKeys++;
        }

        Debug.Log("TEST MODE: Spawned " + spawnedKeys + " keys.");

        yield break;*/


        
        // ==========================================
        // NORMAL MODE: RANDOM KEY SPAWNING
        // ==========================================

        // Find all generated rooms
        Transform[] allObjects =
            FindObjectsOfType<Transform>();

        List<Transform> rooms =
            new List<Transform>();

        foreach (Transform obj in allObjects)
        {
            if (obj.name == "Spawns")
            {
                string roomName = obj.parent.name;

                if (roomName.StartsWith("SpecialTemplate") ||
                    roomName.StartsWith("RoomTemplate"))
                {
                    continue;
                }

                rooms.Add(obj);
            }
        }

        if (rooms.Count < keysToSpawn)
        {
            Debug.LogError(
                "Not enough rooms with key spawn points! Found: "
                + rooms.Count
            );

            yield break;
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            int randomIndex = Random.Range(i, rooms.Count);

            Transform temp = rooms[i];
            rooms[i] = rooms[randomIndex];
            rooms[randomIndex] = temp;
        }

        for (int i = 0; i < keysToSpawn; i++)
        {
            Transform spawnFolder = rooms[i];

            List<Transform> spawnPoints =
                new List<Transform>();

            foreach (Transform point in spawnFolder)
            {
                spawnPoints.Add(point);
            }

            if (spawnPoints.Count == 0)
                continue;

            Transform chosenSpawn =
                spawnPoints[Random.Range(0, spawnPoints.Count)];

            Instantiate(
                keyPrefab,
                chosenSpawn.position,
                chosenSpawn.rotation
            );

            Debug.Log(
                "Key spawned in: " + spawnFolder.parent.name
            );
        }
    }
}
