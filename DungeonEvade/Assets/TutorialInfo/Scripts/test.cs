using UnityEngine;

public class FOVTest : MonoBehaviour
{
    void Start()
    {
        GetComponent<Camera>().fieldOfView = 30;
    }
}
