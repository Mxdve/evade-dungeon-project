using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursor : MonoBehaviour
{
    private RectTransform cursorRect;

    void Start()
    {
        cursorRect = GetComponent<RectTransform>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            cursorRect.position = mousePosition;
        }
    }
}
