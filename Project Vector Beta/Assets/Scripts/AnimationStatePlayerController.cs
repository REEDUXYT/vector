using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Mirror;

public class AnimationStatePlayerController : NetworkBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public Rig weaponAiming;

    int isRifleWalkingHash;
    int isRifleRunningHash;
    int isRifleStrafingLeftHash;
    int isRifleStrafingRightHash;
    int isRifleWalkingBackwardsHash;
    int isRifleJumpingHash;

    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponent<Animator>();
        isRifleWalkingHash = Animator.StringToHash("isRifleWalking");
        isRifleRunningHash = Animator.StringToHash("isRifleRunning");
        isRifleStrafingLeftHash = Animator.StringToHash("isRifleStrafingLeft");
        isRifleStrafingRightHash = Animator.StringToHash("isRifleStrafingRight");
        isRifleWalkingBackwardsHash = Animator.StringToHash("isRifleWalkingBackwards");
        isRifleJumpingHash = Animator.StringToHash("isRifleJumping");
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        bool isRifleWalking = animator.GetBool(isRifleWalkingHash);
        bool isRifleRunning = animator.GetBool(isRifleRunningHash);
        bool isRifleStrafingLeft = animator.GetBool(isRifleStrafingLeftHash);
        bool isRifleStrafingRight = animator.GetBool(isRifleStrafingRightHash);
        bool isRifleWalkingBackwards = animator.GetBool(isRifleWalkingBackwardsHash);
        bool isRifleJumping = animator.GetBool(isRifleJumpingHash);

        bool forwardKeypress = Input.GetKey("w");
        bool runningKeypress = Input.GetKey("left shift");
        bool leftKeypress = Input.GetKey("a");
        bool rightKeypress = Input.GetKey("d");
        bool backwardKeypress = Input.GetKey("s");
        bool jumpKeypress = Input.GetKey("space");

        IEnumerator SmoothRig(float start, float end)
        {
            float elapsedTime = 0;
            float waitTime = 0.2f;
            while (elapsedTime < waitTime)
            {
                weaponAiming.weight = Mathf.Lerp(start, end, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        if (!isRifleWalking && forwardKeypress)
        {
            animator.SetBool(isRifleWalkingHash, true);
        }

        if (isRifleWalking && !forwardKeypress)
        {
            animator.SetBool(isRifleWalkingHash, false);
        }

        if (!isRifleRunning && (forwardKeypress && runningKeypress))
        {
            animator.SetBool(isRifleRunningHash, true);
            //Smooths Rig Weight between 1 and 0.
            StartCoroutine(SmoothRig(1, 0));
            //weaponAiming.weight = 0;
        }

        if (isRifleRunning && (!forwardKeypress || !runningKeypress))
        {
            animator.SetBool(isRifleRunningHash, false);
            //Smooths Rig Weight between 0 and 1.
            StartCoroutine(SmoothRig(0, 1));
            //weaponAiming.weight = 1;
        }

        if (!isRifleStrafingLeft && leftKeypress && !runningKeypress)
        {
            animator.SetBool(isRifleStrafingLeftHash, true);
        }

        if (isRifleStrafingLeft && !leftKeypress)
        {
            animator.SetBool(isRifleStrafingLeftHash, false);
        }

        if (!isRifleStrafingRight && rightKeypress && !runningKeypress)
        {
            animator.SetBool(isRifleStrafingRightHash, true);
        }

        if (isRifleStrafingRight && !rightKeypress)
        {
            animator.SetBool(isRifleStrafingRightHash, false);
        }

        if (!isRifleWalkingBackwards && backwardKeypress)
        {
            animator.SetBool(isRifleWalkingBackwardsHash, true);
        }

        if (isRifleWalkingBackwards && !backwardKeypress)
        {
            animator.SetBool(isRifleWalkingBackwardsHash, false);
        }

        if (!isRifleJumping && jumpKeypress)
        {
            animator.SetBool(isRifleJumpingHash, true);
        }
    }
}
