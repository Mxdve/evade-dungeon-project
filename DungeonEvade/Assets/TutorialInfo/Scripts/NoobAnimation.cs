using UnityEngine;
using StarterAssets;

public class NoobAnimation : MonoBehaviour
{
    public Transform head;
    public Transform torso;

    public Transform leftShoulder;
    public Transform rightShoulder;

    public Transform leftHip;
    public Transform rightHip;

    public CharacterController controller;
    public StarterAssetsInputs input;

    public float swingSpeed = 8f;
    public float swingAmount = 25f;

    private Vector3 originalTorsoPos;
    private Quaternion originalTorsoRot;
    private Quaternion originalHeadRot;

    void Start()
    {
        originalTorsoPos = torso.localPosition;
        originalTorsoRot = torso.localRotation;
        originalHeadRot = head.localRotation;
    }

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(
            controller.velocity.x,
            0f,
            controller.velocity.z
        );

        float currentSpeed = horizontalVelocity.magnitude;

        // Debug.Log("Speed: " + currentSpeed);

        if (input.move != Vector2.zero)
        {
            float swing =
                Mathf.Sin(Time.time * swingSpeed)
                * swingAmount;

            leftShoulder.localRotation =
                Quaternion.Euler(swing, 0, 0);

            rightShoulder.localRotation =
                Quaternion.Euler(-swing, 0, 0);

            leftHip.localRotation =
                Quaternion.Euler(-swing, 0, 0);

            rightHip.localRotation =
                Quaternion.Euler(swing, 0, 0);

            float bob =
                Mathf.Abs(
                    Mathf.Sin(Time.time * swingSpeed)
                ) * 0.05f;

            torso.localPosition =
                originalTorsoPos +
                Vector3.up * bob;

            torso.localRotation =
                Quaternion.Euler(
                    0,
                    Mathf.Sin(Time.time * swingSpeed) * 3f,
                    0
                );

            head.localRotation =
                Quaternion.Euler(
                    Mathf.Sin(Time.time * swingSpeed) * 4f,
                    0,
                    0
                );
        }
        else
        {
            leftShoulder.localRotation = Quaternion.identity;
            rightShoulder.localRotation = Quaternion.identity;

            leftHip.localRotation = Quaternion.identity;
            rightHip.localRotation = Quaternion.identity;

            torso.localPosition = originalTorsoPos;
            torso.localRotation = originalTorsoRot;
            head.localRotation = originalHeadRot;
        }
    }
}
