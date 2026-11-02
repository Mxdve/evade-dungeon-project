using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    [Header("Player Settings")]
    public CharacterController controller;

    [Header("Footstep Sounds")]
    public AudioClip steelSound;
    public AudioClip tileSound;
    public AudioClip woodSound;
    public AudioClip carpetSound;

    [Header("Playback Speed")]
    public float walkPitch = 1f;
    public float runPitch = 1.4f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(
            controller.velocity.x,
            0f,
            controller.velocity.z
        );

        float speed = horizontalVelocity.magnitude;

        if (speed < 0.1f || !controller.isGrounded)
        {
            audioSource.Stop();
            return;
        }

        bool running = speed > 7f;

        audioSource.pitch = running ? runPitch : walkPitch;

        RaycastHit hit;

        Vector3 rayOrigin = controller.bounds.center;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out hit,
            controller.height / 2f + 0.3f))
        {
            AudioClip floorSound = null;

            if (hit.collider.CompareTag("Steel"))
                floorSound = steelSound;
            else if (hit.collider.CompareTag("Tile"))
                floorSound = tileSound;
            else if (hit.collider.CompareTag("Wood"))
                floorSound = woodSound;
            else if (hit.collider.CompareTag("Carpet"))
                floorSound = carpetSound;

            if (floorSound != null)
            {
                if (audioSource.clip != floorSound)
                {
                    audioSource.clip = floorSound;
                    audioSource.Play();
                }
                else if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }
        }
    }
}
