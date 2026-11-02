using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public void Open()
    {
        PlayerDeathManager deathManager =
            FindObjectOfType<PlayerDeathManager>();

        StarterAssets.FirstPersonController player =
            FindObjectOfType<StarterAssets.FirstPersonController>();

        TreasureMessageManager messageManager =
            FindObjectOfType<TreasureMessageManager>();

        int reward = Random.Range(0, 4);

        switch (reward)
        {
            case 0:
                int oldLives = deathManager.LivesRemaining;
                deathManager.AddLife();

                if (deathManager.LivesRemaining > oldLives)
                {
                    messageManager.ShowMessage("EXTRA LIFE! +1 LIFE");
                }
                else
                {
                    messageManager.ShowMessage("LIFE FULL! NO EXTRA LIFE");
                }
                break;

            case 1:
                messageManager.ShowMessage("TRAP! YOU LOST A LIFE!");
                deathManager.RemoveLife();
                break;

            case 2:
                messageManager.ShowMessage("SPEED BOOST! 10 SECONDS!");
                player.ApplySpeedEffect(1.5f, 10f);
                break;

            case 3:
                messageManager.ShowMessage("SLOWED DOWN! 10 SECONDS!");
                player.ApplySpeedEffect(0.5f, 10f);
                break;
        }

        Destroy(gameObject);
    }
}
