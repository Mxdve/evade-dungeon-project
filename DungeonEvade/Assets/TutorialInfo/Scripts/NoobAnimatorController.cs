using UnityEngine;
using StarterAssets;

public class NoobAnimatorController : MonoBehaviour
{
    public Animator animator;
    public CharacterController controller;
    public StarterAssetsInputs input;
    public FirstPersonController firstPersonController;

    private bool jumpPressedLastFrame = false;

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(
            controller.velocity.x,
            0f,
            controller.velocity.z
        );

        float speed = horizontalVelocity.magnitude;

        animator.SetFloat("Speed", speed);

        animator.SetBool("Grounded", firstPersonController.Grounded);
        animator.SetFloat("VerticalVelocity", controller.velocity.y);

        Debug.Log("Grounded: " + firstPersonController.Grounded);

        if (input.jump &&
            !jumpPressedLastFrame &&
            firstPersonController.Grounded)
        {
            Debug.Log("JUMP TRIGGERED");
            animator.SetTrigger("Jump");
        }

        jumpPressedLastFrame = input.jump;
    }
}
