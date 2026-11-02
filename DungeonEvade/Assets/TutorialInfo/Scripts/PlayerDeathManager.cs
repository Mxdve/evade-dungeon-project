
using System.Collections;
using UnityEngine;

public class PlayerDeathManager : MonoBehaviour
{
    [Header("Lives")]
    public int startingLives = 3;

    [Header("Respawn")]
    public float invincibilityDuration = 3f;
    public CanvasGroup caughtScreen;
    public float caughtMessageDuration = 2f;

    [Header("Game Over")]
    public CanvasGroup lostScreen;
    public AudioSource warehouseAmbience;
    public float lostScreenWait = 2f;
    public float fadeDuration = 2f;
    public AudioSource lostSound;

    public int LivesRemaining { get; private set; }
    public bool IsInvincible { get; private set; }

    private Transform player;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private void Start()
    {
        LivesRemaining = startingLives;
    }

    public void SetupPlayer(GameObject playerObject, Transform spawnPoint)
    {
        player = playerObject.transform;

        spawnPosition = spawnPoint.position;
        spawnRotation = spawnPoint.rotation;
    }

    public void PlayerDied()
    {
        if (IsInvincible || LivesRemaining <= 0)
            return;

        LivesRemaining--;

        Debug.Log("Player died! Lives remaining: " + LivesRemaining);

        if (LivesRemaining > 0)
        {
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            Debug.Log("GAME OVER!");
            StartCoroutine(GameOverSequence());
        }
    }

    public void AddLife()
    {
        if (LivesRemaining < startingLives)
        {
            LivesRemaining++;

            Debug.Log("Extra life collected! Lives: " + LivesRemaining);
        }
        else
        {
            Debug.Log("Already at maximum lives!");
        }
    }

    public void RemoveLife()
    {
        PlayerDied();
    }

    private IEnumerator RespawnPlayer()
    {
        IsInvincible = true;

        // Pause the game.
        Time.timeScale = 0f;

        // Show the caught screen.
        caughtScreen.gameObject.SetActive(true);
        caughtScreen.alpha = 1f;

        // Keep the message visible for 2 real-time seconds.
        yield return new WaitForSecondsRealtime(caughtMessageDuration);

        // Find the player's CharacterController.
        CharacterController controller =
            player.GetComponent<CharacterController>();

        // Temporarily disable the controller to move the player.
        if (controller != null)
            controller.enabled = false;

        // Move the player back to the starting room.
        player.position = spawnPosition;
        player.rotation = spawnRotation;

        if (controller != null)
            controller.enabled = true;

        // Hide the caught screen.
        caughtScreen.gameObject.SetActive(false);

        // Resume the game.
        Time.timeScale = 1f;

        Debug.Log("Player respawned! Invincible for 3 seconds.");

        // Wait until invincibility expires.
        yield return new WaitForSeconds(invincibilityDuration);

        IsInvincible = false;

        Debug.Log("Player is vulnerable again!");
    }


    private IEnumerator GameOverSequence()
    {
        // Stop the warehouse ambience.
        if (warehouseAmbience != null)
        {
            warehouseAmbience.Stop();
        }

        // Show the YOU LOST screen.
        lostScreen.gameObject.SetActive(true);
        lostScreen.alpha = 0f;

        // Play the game over sound.
        if (lostSound != null)
        {
            lostSound.Play();
        }

        // Fade the YOU LOST screen in.
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            lostScreen.alpha = Mathf.Clamp01(
                elapsed / fadeDuration
            );

            yield return null;
        }

        lostScreen.alpha = 1f;

        // Keep the screen visible for 2 seconds.
        yield return new WaitForSeconds(lostScreenWait);

        // Exit the game.
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
