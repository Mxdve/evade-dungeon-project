using UnityEngine;
using TMPro;
using System.Collections;

public class HintMessage : MonoBehaviour
{
    public TextMeshProUGUI hintText;

    void Start()
    {
        StartCoroutine(HideText());
    }

    IEnumerator HideText()
    {
        yield return new WaitForSeconds(2f);

        hintText.gameObject.SetActive(false);
    }
}
