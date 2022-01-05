using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
    Camera cam;
    Ray ray;
    RaycastHit hitInfo;
   
    void Start()
    {
        cam = GetComponentInParent<Camera>();
    }

    void Update()
    {
        ray.origin = cam.transform.position;
        ray.direction = cam.transform.forward;
        if (Physics.Raycast(ray, out hitInfo))
        {
            transform.position = hitInfo.point;
        }
        else
        {
            transform.position = ray.origin + ray.direction * 1000.0f;
        }
    }
}
