using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int isStrafingLeftHash;
    int isStrafingRightHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isStrafingLeftHash = Animator.StringToHash("isStrafingLeft");
        isStrafingRightHash = Animator.StringToHash("isStrafingRight");
    }

    // Update is called once per frame
    void Update()
    {
        bool isrunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isStrafingLeft = animator.GetBool(isStrafingLeftHash);
        bool isStrafingRight = animator.GetBool(isStrafingRightHash);
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");

        if (!isWalking && forwardPressed)
        {
            animator.SetBool(isWalkingHash, true);
        }

        if (isWalking && !forwardPressed)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if (!isrunning && (forwardPressed && runPressed))
        {
            animator.SetBool(isRunningHash, true);
        }

        if (isrunning && (!forwardPressed || !runPressed))
        {
            animator.SetBool(isRunningHash, false);
        }

        if (!isStrafingLeft && leftPressed)
        {
            animator.SetBool(isStrafingLeftHash, true);
        }

        if (isStrafingLeft && !leftPressed)
        {
            animator.SetBool(isStrafingLeftHash, false);
        }

        if (!isStrafingRight && rightPressed)
        {
            animator.SetBool(isStrafingRightHash, true);
        }

        if (isStrafingRight && !rightPressed)
        {
            animator.SetBool(isStrafingRightHash, false);
        }

    }
}
