using UnityEngine;

public class WarehouseGenerator : MonoBehaviour
{
    public GameObject[] templates;

    private void Start()
    {
        int index =
            Random.Range(
                0,
                templates.Length
            );

        Instantiate(
            templates[index],
            //templates[5],
            Vector3.zero,
            Quaternion.identity
        );
    }
}
