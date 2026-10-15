using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class FOVZoom : MonoBehaviour
{
    public CinemachineCamera cineCam;

    public float zoomSpeed = 0.05f;
    public float minFOV = 40f;
    public float maxFOV = 120f;

    void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            cineCam.Lens.FieldOfView -= scroll * zoomSpeed;
            cineCam.Lens.FieldOfView =
                Mathf.Clamp(cineCam.Lens.FieldOfView, minFOV, maxFOV);
        }
    }
}
