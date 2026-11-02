using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu")]
    public CanvasGroup menuCanvas;

    [Header("Game Objects")]
    public GameObject warehouseGenerator;
    public FirstPersonController playerController;
    public PlayerInteraction playerInteraction;
    public Image crosshair;
    public GameObject customCursor;

    [Header("Audio")]
    public AudioSource warehouseAmbience;
    public AudioSource menuMusic;

    public void StartGame()
    {
        // Hide the menu
        menuCanvas.alpha = 0f;
        menuCanvas.interactable = false;
        menuCanvas.blocksRaycasts = false;
        customCursor.SetActive(false);
        menuMusic.Stop();

        // Start warehouse generation
        warehouseGenerator.SetActive(true);

        // Enable player controls
        playerController.enabled = true;
        playerInteraction.enabled = true;

        // Show the crosshair
        crosshair.gameObject.SetActive(true);

        // Start warehouse ambience
        warehouseAmbience.Play();

        // Lock and hide the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
