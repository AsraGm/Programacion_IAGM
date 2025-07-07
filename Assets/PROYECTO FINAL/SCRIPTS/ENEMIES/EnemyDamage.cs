using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 1;
    public GameObject deathParticlesPrefab; 
    public GameObject hitParticlesPrefab; 
    private Spawn Spawn;
    private bool isDead = false;
   // private REQUIREMENTS requirements;

    private void Start()
    {
        Spawn = FindObjectOfType<Spawn>(); // Busca el SpawnManager al inicio
        if (Spawn == null)
        {

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            if (hitParticlesPrefab != null)
            {
                Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
            }

            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.GetDamage(damage);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }
        //if (requirements != null)
        //{
        //    requirements.ObjetoDestruido();
        //}
        else
        {
            Destroy(gameObject);
        }
    }
}