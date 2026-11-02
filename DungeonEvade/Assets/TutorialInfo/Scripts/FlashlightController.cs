using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    public Transform rightShoulder;
    public GameObject flashlight;

    [Header("Raised Shoulder Pose")]
    public Vector3 raisedPosition = new Vector3(0.367f, 1.124f, 0f);
    public Vector3 raisedRotation = new Vector3(-149.977f, 0f, 0f);

    [Header("Animation Settings")]
    public float transitionSpeed = 5f;

    [Header("Camera and Light")]
    public Transform playerCamera;
    public Light flashlightSpotlight;

    private bool flashlightOn = false;
    private float flashlightWeight = 0f;

    private void Start()
    {
        if (rightShoulder == null || flashlight == null)
        {
            Debug.LogError("FlashlightController is missing a reference!");
            enabled = false;
            return;
        }

        flashlight.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            flashlightOn = !flashlightOn;
            flashlight.SetActive(flashlightOn);
        }

        float targetWeight = flashlightOn ? 1f : 0f;

        flashlightWeight = Mathf.MoveTowards(
            flashlightWeight,
            targetWeight,
            Time.deltaTime * transitionSpeed
        );
    }

    private void LateUpdate()
    {
        // Get the current animated shoulder pose
        Vector3 animatedPosition = rightShoulder.localPosition;
        Quaternion animatedRotation = rightShoulder.localRotation;

        // Blend between the animation and flashlight pose
        rightShoulder.localPosition = Vector3.Lerp(
            animatedPosition,
            raisedPosition,
            flashlightWeight
        );

        rightShoulder.localRotation = Quaternion.Slerp(
            animatedRotation,
            Quaternion.Euler(raisedRotation),
            flashlightWeight
        );

        if (flashlightOn && flashlightSpotlight != null)
        {
            flashlightSpotlight.transform.rotation = playerCamera.rotation;
        }
    }

    private void OnDisable()
    {
        if (flashlight != null)
            flashlight.SetActive(false);
    }
}
