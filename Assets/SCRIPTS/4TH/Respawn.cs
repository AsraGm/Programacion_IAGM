using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform respawnPoint;

    public void TeleportPlayer(Transform playerTransform)
    {
        if (respawnPoint != null)
        {
            playerTransform.position = respawnPoint.position;

            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Debug.Log("Jugador respawneado en posición fija");
        }
        else
        {
            Debug.LogError("Asigna un respawnPoint en el Inspector");
        }
    }
}