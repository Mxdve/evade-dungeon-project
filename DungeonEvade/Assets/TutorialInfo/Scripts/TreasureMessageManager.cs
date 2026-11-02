using System.Collections;
using TMPro;
using UnityEngine;

public class TreasureMessageManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float messageDuration = 2f;

    private Coroutine messageCoroutine;

    public void ShowMessage(string message)
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(DisplayMessage(message));
    }

    private IEnumerator DisplayMessage(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        messageText.gameObject.SetActive(false);
        messageCoroutine = null;
    }
}
