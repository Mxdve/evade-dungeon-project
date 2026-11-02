using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public Image crosshair;
    public CameraToggle cameraToggle;

    public AudioClip keyPickupSound;

    public float firstPersonDistance = 3f;
    public float thirdPersonDistance = 10f;

    private KeyPickup currentKey;
    private DoorLock currentLock;
    private NormalDoor currentDoor;
    private TreasureChest currentChest;

    private Collider[] playerColliders;

    private bool hasKey = false;

    void Start()
    {
        playerColliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        currentKey = null;
        currentLock = null;
        currentDoor = null;
        currentChest = null;

        float currentDistance = cameraToggle.thirdPerson
            ? thirdPersonDistance
            : firstPersonDistance;

        // Temporarily disable player's colliders
        foreach (Collider col in playerColliders)
        {
            col.enabled = false;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            currentDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide))
        {
            //Debug.Log("Raycast hit: " + hit.collider.name);
            currentDoor = hit.collider.GetComponentInParent<NormalDoor>();
            currentKey = hit.collider.GetComponentInParent<KeyPickup>();
            currentLock = hit.collider.GetComponentInParent<DoorLock>();
            currentChest = hit.collider.GetComponentInParent<TreasureChest>();
        }

        // Re-enable player colliders
        foreach (Collider col in playerColliders)
        {
            col.enabled = true;
        }

        // Update crosshair
        if ((!hasKey && currentKey != null) ||
            (hasKey && currentLock != null) ||
            currentDoor != null ||
            currentChest != null)
        {
            crosshair.color = Color.green;
        }
        else
        {
            crosshair.color = Color.white;
        }

        // Press E to interact
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Open a normal door
            if (currentDoor != null)
            {
                currentDoor.Open(transform.position);
            }

            // Pick up a key
            else if (!hasKey && currentKey != null)
            {
                hasKey = true;

                if (keyPickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(
                        keyPickupSound,
                        transform.position
                    );
                }

                Destroy(currentKey.gameObject);

                Debug.Log("Picked up a key!");
            }

            // Unlock a lock
            else if (hasKey && currentLock != null)
            {
                hasKey = false;
                currentLock.Unlock();

                Debug.Log("Unlocked a lock!");
            }

            // Open a treasure chest
            else if (currentChest != null)
            {
                currentChest.Open();
            }
        }
    }
}
