using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class GameEndingManager : MonoBehaviour
{
    [Header("Ending UI")]
    public GameObject endingScreen;
    public Image fadeImage;
    public CanvasGroup endingCanvasGroup;

    [Header("Player")]
    public FirstPersonController playerController;
    public PlayerInteraction playerInteraction;

    [Header("UI Elements")]
    public Image crosshair;
    public GameObject customCursor;

    [Header("Audio")]
    public AudioSource warehouseAmbience;
    public AudioSource endingMusic;

    [Header("Fade Settings")]
    public float fadeDuration = 3f;
    public float endingFadeDuration = 2f;

    private bool endingStarted = false;

    private void Start()
    {
        endingScreen.SetActive(false);

        fadeImage.gameObject.SetActive(true);
        SetFadeAlpha(0f);
    }

    public void StartEnding()
    {
        if (endingStarted)
            return;

        endingStarted = true;

        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        // Stop player movement and interaction.
        playerController.enabled = false;
        playerInteraction.enabled = false;

        // Hide the gameplay crosshair.
        crosshair.gameObject.SetActive(false);

        // Fade out the warehouse ambience.
        float startingVolume = warehouseAmbience.volume;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float progress = elapsed / fadeDuration;

            SetFadeAlpha(progress);

            warehouseAmbience.volume =
                Mathf.Lerp(startingVolume, 0f, progress);

            yield return null;
        }

        SetFadeAlpha(1f);
        warehouseAmbience.Stop();

        // Show the ending screen.
        endingScreen.SetActive(true);

        // Start ending music immediately.
        endingMusic.Play();

        // Fade in the ending screen.
        endingCanvasGroup.alpha = 0f;

        float endingElapsed = 0f;

        while (endingElapsed < endingFadeDuration)
        {
            endingElapsed += Time.deltaTime;

            endingCanvasGroup.alpha =
                Mathf.Clamp01(endingElapsed / endingFadeDuration);

            yield return null;
        }

        endingCanvasGroup.alpha = 1f;

        // Show the custom cursor.
        customCursor.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
