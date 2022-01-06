using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLook : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement playerMovement;
    
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] Transform camHolder;
    [SerializeField] Transform orientation;

    RaycastWeapon weapon;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        weapon = GetComponentInChildren<RaycastWeapon>();
    }


    private void Update()
    {
        MyInput();
        Shoot();

        camHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation, playerMovement.tilt);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    /*private void LateUpdate()
    {
        Shoot();
    }*/
    void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    void Shoot()
    {
        if (Input.GetButtonDown("Fire1") && (playerMovement.isSprinting == false))
        {
            weapon.StartFiring();
            //Debug.Log("The Player is firing.");
        }
        if (weapon.isFiring)
        {
            weapon.UpdateFiring(Time.deltaTime);
        }
        weapon.UpdateBullets(Time.deltaTime);
        
        if (Input.GetButtonUp("Fire1") || (playerMovement.isSprinting == true))
        {
            weapon.StopFiring();
            //Debug.Log("The Player stopped firing.");
        }
    }
}