using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Player.Weapon;

namespace Player
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private Weapon[] weapons;
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private float detectionRange = 5f;
        [SerializeField] private LayerMask weaponLayer;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private Transform pointray;

        private List<Weapon> weaponList = new List<Weapon>();
        private int currentWeaponIndex = 0;
        private Weapon actualWeapon;
        private Action Shoot;

        RaycastHit hit;

        private void Start()
        {

            if (weapons != null && weapons.Length < 1)
            {
                Debug.LogWarning("[WeaponHandler] No hay armas asignadas al WeaponHandler.");
            }
        }

        private void Update()
        {
            HandleWeaponSwitch();
            HandleWeaponPickup();

            // Solo dispara si tenemos un arma equipada
            if (actualWeapon != null)
            {
                Shoot?.Invoke();
                actualWeapon?.Reload();
            }
        }

        private void HandleWeaponSwitch()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                int previousIndex = currentWeaponIndex;
                if (scroll > 0)
                {
                    currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
                }
                else
                {
                    currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
                }

                if (previousIndex != currentWeaponIndex)
                {
                    EquipWeapon(currentWeaponIndex);
                }
            }
        }

        private void EquipWeapon(int index)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    weapons[i].gameObject.SetActive(i == index);
                }
                else
                {
                    Debug.LogWarning($"[WeaponHandler] El arma en el índice {i} está vacía.");
                }
            }

            if (index < 0 || index >= weapons.Length || weapons[index] == null)
            {
                actualWeapon = null;
                return;
            }

            actualWeapon = weapons[index];

            // Configurar el tipo de disparo
            switch (actualWeapon.fireType)
            {
                case FireType.Automatic:
                    Shoot = AutomaticShoot;
                    break;
                case FireType.SemiAutomatic:
                    Shoot = SemiAutomaticShoot;
                    break;
                default:
                    break;
            }

            // Actualizar UI de munición
            if (ammoText != null)
            {
                ammoText.text = $"{actualWeapon.currentAmmo}/{actualWeapon.ammo}";
            }
        }

        private void AutomaticShoot()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                actualWeapon.Shoot();
            }
        }

        private void SemiAutomaticShoot()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                actualWeapon.Shoot();
            }
        }

        private void HandleWeaponPickup()
        {
            if (DetectionWeapon())
            {

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Weapon pickedWeapon = hit.collider.GetComponent<Weapon>();

                    if (pickedWeapon != null)
                    {
                        if (!weaponList.Contains(pickedWeapon))
                        {

                            weaponList.Add(pickedWeapon);
                            weapons = weaponList.ToArray();
                         

                            pickedWeapon.transform.SetParent(weaponHolder);
                            pickedWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                            pickedWeapon.GetComponent<Collider>().enabled = false;
                          

                            currentWeaponIndex = weapons.Length - 1;
                            EquipWeapon(currentWeaponIndex);
                        }
                        else
                        {
                            Debug.Log("[WeaponHandler] Esta arma ya está en la colección");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[WeaponHandler] Objeto detectado no tiene componente Weapon");
                    }
                }
            }
        }

        private bool DetectionWeapon()
        {
            bool weaponDetected = Physics.Raycast(pointray.position, transform.forward, out hit, detectionRange, weaponLayer);
            if (weaponDetected)
            {
                Debug.DrawRay(pointray.position, transform.forward * detectionRange, Color.green);
            }
            else
            {
                Debug.DrawRay(pointray.position, transform.forward * detectionRange, Color.red);
            }
            return weaponDetected;
        }

        private void OnDrawGizmos()
        {
            if (pointray == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(pointray.position, transform.forward * detectionRange);
        }
    }
}