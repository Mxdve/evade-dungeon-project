using UnityEngine;

public class NormalDoor : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Door Settings")]
    public bool invertSwingDirection = false;

    [Header("Door Sound")]
    public AudioClip openSound;

    private bool isOpen = false;
    private Quaternion openRotation;

    public void Open(Vector3 playerPosition)
    {
        if (isOpen)
            return;

        isOpen = true;

        Vector3 directionToPlayer = playerPosition - transform.position;

        float side = Vector3.Dot(transform.forward, directionToPlayer);

        float direction = side >= 0 ? -1f : 1f;

        if (invertSwingDirection)
        {
            direction *= -1f;
        }

        openRotation = transform.rotation *
            Quaternion.Euler(0f, openAngle * direction, 0f);

        if (openSound != null)
        {
            AudioSource.PlayClipAtPoint(
                openSound,
                transform.position,
                1f
            );
        }
    }

    void Update()
    {
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                openRotation,
                Time.deltaTime * openSpeed
            );
        }
    }
}
