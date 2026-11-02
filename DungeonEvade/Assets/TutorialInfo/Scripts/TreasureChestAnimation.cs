using UnityEngine;

public class TreasureChestAnimation : MonoBehaviour
{
    public float rotationSpeed = 60f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.15f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        // Spin the chest
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Make the chest gently float
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.localPosition = startPosition + Vector3.up * newY;
    }
}
