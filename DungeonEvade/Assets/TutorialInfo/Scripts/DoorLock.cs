using UnityEngine;

public class DoorLock : MonoBehaviour
{
    public AudioClip unlockSound;

    public void Unlock()
    {
        // Play the unlock sound
        if (unlockSound != null)
        {
            AudioSource audioSource = new GameObject("UnlockSound").AddComponent<AudioSource>();

            audioSource.clip = unlockSound;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
            audioSource.Play();

            Destroy(audioSource.gameObject, unlockSound.length);
        }

        // Tell the exit door that a lock was removed.
        ExitDoor exitDoor = FindObjectOfType<ExitDoor>();

        if (exitDoor != null)
        {
            exitDoor.LockRemoved();
        }

        Destroy(gameObject);
    }
}
