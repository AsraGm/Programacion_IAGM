using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField] protected internal float range; // alcance de el arma

    [SerializeField] protected LayerMask detection; // a que se le puede disparar

    protected RaycastHit target;

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward * range, out target, range, detection))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                target.collider.GetComponent<IInteractable>().Interact();
                Destroy(target.collider.gameObject); // Destruye el objeto al interactuar
            }
        }
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * range);
    }
}
