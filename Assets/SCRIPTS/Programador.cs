using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Videojuegos
{

    public class Programador : MonoBehaviour
    {
        Weapon arma;
       Weapon arma2;

        public void CrearControlDeMovimiento()
        {
            arma.Shoot();
            arma.Reload();

            

        }
    }
}

