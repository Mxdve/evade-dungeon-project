
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuParallax : MonoBehaviour
{
    public float movementAmount = 20f;
    public float smoothSpeed = 5f;

    private RectTransform rectTransform;
    private Vector2 startingPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startingPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector2 screenCenter = new Vector2(
            Screen.width / 2f,
            Screen.height / 2f
        );

        Vector2 mouseOffset = (mousePosition - screenCenter) / screenCenter;

        Vector2 targetPosition = startingPosition + new Vector2(
            mouseOffset.x * movementAmount,
            mouseOffset.y * movementAmount
        );

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.deltaTime * smoothSpeed
        );
    }
}
