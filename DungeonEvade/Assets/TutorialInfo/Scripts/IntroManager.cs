
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [Header("Intro")]
    public GameObject introCanvas;
    public VideoPlayer videoPlayer;
    public Button skipButton;

    [Header("Main Menu")]
    public CanvasGroup mainMenuCanvas;
    public GameObject customCursor;
    public AudioSource menuMusic;

    [Header("Fade Settings")]
    public float menuFadeDuration = 2f;

    private bool introFinished = false;

    private void Start()
    {
        // Keep the main menu hidden.
        mainMenuCanvas.alpha = 0f;
        mainMenuCanvas.interactable = false;
        mainMenuCanvas.blocksRaycasts = false;

        // Show the custom cursor during the intro.
        customCursor.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        // Show the intro.
        introCanvas.SetActive(true);

        // Connect the video-finished event.
        videoPlayer.loopPointReached += OnVideoFinished;

        // Connect the skip button.
        skipButton.onClick.AddListener(SkipIntro);

        // Start playing the video.
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer player)
    {
        FinishIntro();
    }

    public void SkipIntro()
    {
        if (introFinished)
            return;

        videoPlayer.Stop();
        FinishIntro();
    }

    private void FinishIntro()
    {
        if (introFinished)
            return;

        introFinished = true;

        StartCoroutine(ShowMainMenu());
    }

    private IEnumerator ShowMainMenu()
    {
        menuMusic.Play();

        // Hide the intro.
        introCanvas.SetActive(false);

        // Fade in the main menu.
        float elapsed = 0f;

        while (elapsed < menuFadeDuration)
        {
            elapsed += Time.deltaTime;

            mainMenuCanvas.alpha =
                Mathf.Clamp01(elapsed / menuFadeDuration);

            yield return null;
        }

        mainMenuCanvas.alpha = 1f;
        mainMenuCanvas.interactable = true;
        mainMenuCanvas.blocksRaycasts = true;

        // Show the custom cursor.
        customCursor.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }
}
