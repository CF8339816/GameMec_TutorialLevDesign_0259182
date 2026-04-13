using UnityEngine;

public class KillBoxManager : MonoBehaviour
{
    Transform targetCheckPoint;
    public PlayerController playerScript;

    //[SerializeField] private Transform respawnPoint;
    //void Start()
    //{
    //    targetCheckPoint = playerScript.ActiveCheckPoint;

    //}
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerScript = other.GetComponent<PlayerController>();

            if (playerScript == null)
            {
                Debug.LogError("KillBox hit something tagged Player, but tthere is no player controler script!");
                return;
            }

            //Transform targetCheckPoint = playerScript.ActiveCheckPoint;
            CharacterController charCtrlr = other.GetComponent<CharacterController>();

            if (charCtrlr != null && targetCheckPoint != null)
            {

                charCtrlr.enabled = false;


                other.transform.position = targetCheckPoint.position;


                //playerScript.Health = 1;
                playerScript.ResetVelocity();

                charCtrlr.enabled = true;
            }
        }
    }
}