
using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;

    [Header("Warehouse")]
    public TemplateRoomFiller templateRoomFiller;

    [Header("NavMesh")]
    public RuntimeNavMeshBuilder navMeshBuilder;

    [Header("NextBots")]
    public GameObject Nextbot1;
    public GameObject Nextbot2;
    public GameObject Nextbot3;

    [Header("Countdown")]
    public TMP_Text countdownText;
    public int countdownTime = 10;

    [Header("Death System")]
    public PlayerDeathManager deathManager;

    IEnumerator Start()
    {
        Debug.Log("PlayerSpawner started!");

        // Wait for warehouse generation.
        while (!templateRoomFiller.GenerationComplete)
        {
            yield return null;
        }

        while (!navMeshBuilder.NavMeshReady)
        {
            yield return null;
        }

        Debug.Log("Warehouse generation detected!");

        // Find the player's spawn point.
        GameObject playerSpawnPoint =
            GameObject.Find("PlayerSpawnPoint");

        if (playerSpawnPoint == null)
        {
            Debug.LogError("No PlayerSpawnPoint found!");
            yield break;
        }

        // Move player into the starting room.
        player.transform.position =
            playerSpawnPoint.transform.position;

        player.transform.rotation =
            playerSpawnPoint.transform.rotation;

        Debug.Log("Player moved to starting room!");

        // Set up the player's death and respawn system.
        deathManager.SetupPlayer(player, playerSpawnPoint.transform);

        // Find all three NextBot spawn points.
        GameObject botSpawnPoint =
            GameObject.Find("NextbotSpawnPoint");

        GameObject botSpawnPoint2 =
            GameObject.Find("NextbotSpawnPoint2");

        GameObject botSpawnPoint3 =
            GameObject.Find("NextbotSpawnPoint3");

        if (botSpawnPoint == null ||
            botSpawnPoint2 == null ||
            botSpawnPoint3 == null)
        {
            Debug.LogError("One or more NextBot spawn points are missing!");
            yield break;
        }

        Debug.Log("Starting countdown!");

        // Start the countdown.
        countdownText.gameObject.SetActive(true);

        for (int i = countdownTime; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "0";

        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);

        Debug.Log("Spawning all three NextBots!");

        // Spawn all three NextBots.
        SpawnNextBot(Nextbot1, botSpawnPoint);
        SpawnNextBot(Nextbot2, botSpawnPoint2);
        SpawnNextBot(Nextbot3, botSpawnPoint3);

        Debug.Log("All three NextBots spawned! The chase begins.");
    }

    // Spawns a NextBot and assigns the player as its target.
    private void SpawnNextBot(GameObject prefab, GameObject spawnPoint)
    {
        GameObject spawnedBot = Instantiate(
            prefab,
            spawnPoint.transform.position,
            spawnPoint.transform.rotation
        );

        NextBot bot = spawnedBot.GetComponent<NextBot>();

        if (bot != null)
        {
            bot.player = player.transform;
        }

        PlayerDeathCollision collision =
            player.GetComponent<PlayerDeathCollision>();

        if (collision != null)
        {
            collision.deathManager = FindObjectOfType<PlayerDeathManager>();
        }
    }
}
