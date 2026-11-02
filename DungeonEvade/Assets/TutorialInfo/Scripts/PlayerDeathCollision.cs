
using UnityEngine;

public class PlayerDeathCollision : MonoBehaviour
{
    public PlayerDeathManager deathManager;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        NextBot bot = hit.collider.GetComponentInParent<NextBot>();

        if (bot != null)
        {
            deathManager.PlayerDied();
        }
    }
}
