using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public bool isFiring = false;
    public ParticleSystem muzzleFlash;
    public Transform raycastOrigin;
    public Transform raycastDestination;

    Ray ray;
    RaycastHit hitInfo;

    public void StartFiring()
    {
        isFiring = true;
        muzzleFlash.Emit(1);

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;
        if(Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
        }

    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
