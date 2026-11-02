using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public int totalLocks = 5;

    public Transform doorPivot;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private int locksRemaining;
    private bool doorOpening = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        locksRemaining = totalLocks;

        closedRotation = doorPivot.localRotation;

        openRotation = closedRotation *
            Quaternion.Euler(0f, openAngle, 0f);
    }

    public void LockRemoved()
    {
        locksRemaining--;

        Debug.Log("Locks remaining: " + locksRemaining);

        if (locksRemaining <= 0)
        {
            doorOpening = true;
        }
    }

    void Update()
    {
        if (doorOpening)
        {
            doorPivot.localRotation = Quaternion.Slerp(
                doorPivot.localRotation,
                openRotation,
                Time.deltaTime * openSpeed
            );
        }
    }
}
