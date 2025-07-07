
using System.Collections;
using TMPro;
using UnityEngine;


namespace Player
{
    public abstract class Weapon : MonoBehaviour
    {
        public enum FireType
        {
            Automatic,
            SemiAutomatic
        }

        public FireType fireType;

        public string weaponName;

        public int damage; // daño del arma
        public float range; // alcance del arma
        public float fireRate; // cadencia del arma
        public float accuracy; // punteria: Que tanto se mueve el arma o dispara hacia donde apuntas
        public float timeToReload; // tiempo de recarga del arma

        public int currentAmmo; // municion de mi cargador actual
        public int currentMaxAmmo; // capacidad maxima de el cargador
        public int ammo; // municion disponible en la reserva
        public int maxAmmo; // capacidad maxima de mi reserva

        [HideInInspector] public float nextFireTime = 0f; // tiempo entre disparos
        [HideInInspector] public bool isReloading = false; // si el arma esta recargando

        [Header("Bullet")]
        public GameObject bulletPrefab; // prefab de la bala
        public Transform shootPoint; // punto de disparo

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI ammoText;

        [Header("Visual Effects")]
        public ParticleSystem P_Shoot;
        public GameObject bulletHoleDecal;  // Prefab del agujero de bala
        public float decalLifetime = 10f;  // Tiempo antes de destruir el decal

        [Header("Sound Effects")]
        public AudioSource audioSource;
        public AudioClip shootSound;

        public abstract void Shoot(); // Insta

        public virtual void Reload()
        {
            if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < currentMaxAmmo && ammo > 0)
            {
                StartCoroutine(ReloadCoroutine());
            }
        }

        protected void Bullet()
        {
            //Efecto visual de disparo (muzzle flash)
            if (P_Shoot != null)
            {
                P_Shoot.Play();
            }

            // 2. Sonido de disparo
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            // Instanciar bala y detectar impacto
            GameObject bulletInstance = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
            bulletRb.AddForce(shootPoint.forward * range, ForceMode.Impulse);

            // agujero de bala 
        
            BULLETBEHAVIOUR bulletBehaviour = bulletInstance.GetComponent<BULLETBEHAVIOUR>();
            if (bulletBehaviour != null)
            {
                bulletBehaviour.bulletHoleDecal = bulletHoleDecal;
                bulletBehaviour.decalLifetime = decalLifetime;
            }
        }

        protected void Ammotext()
        {
            ammoText.text = $"{currentAmmo}/{ammo}";
        }

        private IEnumerator ReloadCoroutine()
        {
            isReloading = true;

            Debug.Log("Recargando...");
            yield return new WaitForSeconds(timeToReload);

            int bulletsNeeded = currentMaxAmmo - currentAmmo;
            int bulletsToReload = Mathf.Min(bulletsNeeded, ammo);

            currentAmmo += bulletsToReload;
            ammo -= bulletsToReload;

            Debug.Log($"Recarga completa: {currentAmmo}/{ammo}");

            Ammotext();

            isReloading = false;
        }

        public bool CheckAmmo()
        {
            return currentAmmo <= 0 && ammo <= 0;
        }

    }
}

