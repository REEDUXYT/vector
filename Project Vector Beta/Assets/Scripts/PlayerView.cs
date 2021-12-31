using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerView : NetworkBehaviour
{

    [Header("PlayerModel")]
    //Add more fields as necessary, these will get the transforms so that the SkinnedMeshRenders of the components can be found.
    [SerializeField] private Camera cam;
    [SerializeField] Transform playerBody;
    [SerializeField] Transform playerJoints;
    //[SerializeField] Transform playerGun;


    void Start()
    {
        if (!isLocalPlayer)
        {
            cam.gameObject.SetActive(false);
            cam.GetComponent<AudioListener>().enabled = false;

            //Later add a method here that disables the mesh render of the FPS arms.
        }
        else
        {
            //Basically if the player isn't local, set the "Player View" to shadows only of their own body.
            playerBody.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            playerJoints.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            //Later add a method here that enables the mesh render of the FPS arms.
        }
    }
}
