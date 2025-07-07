using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Shooting")]
    public Transform shotPoint;

    public GameObject bulletPrefab;

    public float bulletSpeed = 10f;

    public float tiempoEntreDisparos = 0.5f;

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Disparar con el click izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Instanciar la bala en el punto de disparo
        GameObject bullet = Instantiate(bulletPrefab, shotPoint.position, shotPoint.rotation);

        // Obtener el Rigidbody de la bala
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // Aplicar fuerza a la bala para que se mueva
        if (rb != null)
        {
            rb.velocity = shotPoint.forward * bulletSpeed;
        }
        else
        {
            Debug.LogError("La bala no tiene un Rigidbody.");
        }
    }
}