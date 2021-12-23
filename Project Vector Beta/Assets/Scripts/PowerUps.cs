using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Cylinder") ;
        {
            Destroy(other.gameObject);
            playerMovement.jumpForce = 40f;
            playerMovement.gravityDownForce = 70f;
        }

    }
}

