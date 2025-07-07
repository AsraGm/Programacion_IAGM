using UnityEngine;

public class BULLETBEHAVIOUR : MonoBehaviour
{
    [Header("Collision Settings")]
    public LayerMask groundLayer;
    public float destroyDelay = 0f;

    [Header("Enemy Damage")]
    public bool destroyEnemyOnHit = true;
    public int damage = 100;
    public float pushBackForce = 5f;

    [Header("Bullet Hole Decal")]
    public GameObject bulletHoleDecal;
    [Tooltip("The lifetime of the decal in seconds")]
    public float decalLifetime = 10f;
    public float decalOffset = 0.01f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (destroyEnemyOnHit)
            {
                var refScript = collision.gameObject.GetComponent<TargetReference>();

                // Evita contar el mismo objetivo varias veces
                if (refScript != null && collision.gameObject.GetComponent<AlreadyDestroyed>() == null)
                {
                    collision.gameObject.AddComponent<AlreadyDestroyed>();
                    refScript.myRoom.OnTargetDestroyed(collision.gameObject);
                }
            }
            Destroy(gameObject);
            return;
        }


        // Lógica para superficies
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            // La "Mexicanada": Cambiamos la bala por el decal
            ContactPoint contact = collision.contacts[0];

            // Instanciamos el decal en la posición de la bala
            GameObject decal = Instantiate(bulletHoleDecal,
                                         transform.position,
                                         Quaternion.LookRotation(contact.normal));

            // Ajustamos posición exacta
            decal.transform.position = contact.point + contact.normal * decalOffset;

            // Lo hacemos hijo de la superficie
            decal.transform.parent = collision.transform;

            // Destruimos el decal después del tiempo
            Destroy(decal, decalLifetime);

            // Destruimos la bala inmediatamente
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}