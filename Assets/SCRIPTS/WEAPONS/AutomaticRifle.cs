using UnityEngine;

namespace Player
{
    public class AutomaticRifle : Weapon
    {

        private void Awake()
        {
            fireType = FireType.Automatic;
        }

        public override void Shoot()
        {
            if (Time.time >= nextFireTime && currentAmmo > 0)
            {
                if (isReloading) return; // Si el arma esta recargando no dispara

                nextFireTime = Time.time + 1f / fireRate; // Calcula tiempo entre disparos
                currentAmmo--;
                Bullet(); // Instancia la bala hacia adelante
                Ammotext();
            }
        }
    }
}
