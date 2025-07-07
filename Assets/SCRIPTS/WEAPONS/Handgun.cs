using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Handgun : Weapon
    {
        public override void Shoot()
        {
            if (Time.time >= nextFireTime && currentAmmo > 0)
            {
                if (isReloading) return; // Si el arma esta recargando no dispara
                nextFireTime = Time.time / fireRate; // Calcula tiempo entre disparos
                currentAmmo--;
                Bullet(); // Instancia la bala hacia adelante
                Ammotext();
            }
        }


    }
}