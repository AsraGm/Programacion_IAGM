using UnityEngine;


namespace Player
{
    public class Shotgun : Weapon
    {
        [Header("Shotgun Settings")]
        [SerializeField] private int pellets = 8; // Cuántas balas dispara la escopeta
        [SerializeField] private float spreadAngle = 5f; // Qué tanto se dispersan (en grados)

        public override void Shoot()
        {
            for (int i = 0; i < pellets; i++)
            {
                if (isReloading) return;
                nextFireTime = Time.time / fireRate;
                currentAmmo--;
                Ammotext();

                for (i = 0; i < pellets; i++)
                {

                    Quaternion spreadRotation = Quaternion.Euler(
                        shootPoint.eulerAngles.x + Random.Range(-spreadAngle, spreadAngle),
                        shootPoint.eulerAngles.y + Random.Range(-spreadAngle, spreadAngle),
                        shootPoint.eulerAngles.z
                    );

                    GameObject bulletInstance = Instantiate(bulletPrefab, shootPoint.position, spreadRotation);
                    Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
                    rb.AddForce(bulletInstance.transform.forward * range, ForceMode.Impulse);
                }

            }
        }

    }
}