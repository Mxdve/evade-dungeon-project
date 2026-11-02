using UnityEngine;

public class EscapeTrigger : MonoBehaviour
{
    private GameEndingManager gameEndingManager;
    private bool hasTriggered = false;

    private void Start()
    {
        gameEndingManager =
            FindObjectOfType<GameEndingManager>();

        if (gameEndingManager == null)
        {
            Debug.LogError("GameEndingManager not found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        CharacterController player =
            other.GetComponent<CharacterController>();

        if (player != null)
        {
            hasTriggered = true;

            Debug.Log("PLAYER ESCAPED!");

            if (gameEndingManager != null)
            {
                gameEndingManager.StartEnding();
            }
        }
    }
}
