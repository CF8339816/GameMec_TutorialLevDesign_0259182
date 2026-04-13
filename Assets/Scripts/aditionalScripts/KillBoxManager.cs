using UnityEngine;

public class KillBoxManager : MonoBehaviour
{
    public Transform targetCheckPoint;
  

   
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        // IMPORTANT: If you use a CharacterController, you MUST disable it to teleport
        if (player.TryGetComponent(out CharacterController charCtrlr))
        {
            charCtrlr.enabled = false;
            player.transform.position = targetCheckPoint.position;
            charCtrlr.enabled = true;
        }
        else
        {
            // Standard teleport for objects without CharacterControllers
            player.transform.position = targetCheckPoint.position;
        }

        Debug.Log("Player teleported to: " + targetCheckPoint.name);
    }
}