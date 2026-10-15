using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CameraToggle : MonoBehaviour
{
    private CinemachineThirdPersonFollow thirdPersonFollow;

    private bool thirdPerson = false;

    void Start()
    {
        thirdPersonFollow = GetComponent<CinemachineThirdPersonFollow>();

        // Start in first person
        thirdPersonFollow.CameraDistance = 0f;
        thirdPersonFollow.ShoulderOffset = Vector3.zero;
    }

    void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            thirdPerson = !thirdPerson;

            if (thirdPerson)
            {
                thirdPersonFollow.CameraDistance = 7f;
                thirdPersonFollow.ShoulderOffset = new Vector3(3.5f, 1f, 0f);
            }
            else
            {
                thirdPersonFollow.CameraDistance = 0f;
                thirdPersonFollow.ShoulderOffset = Vector3.zero;
            }
        }
    }
}
