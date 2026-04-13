using UnityEngine;
using System.Collections;

public class SpringLauncher : MonoBehaviour
{
    [Header("Launch Settings")]
    public float launchForce = 35f; 
    public float gliderDelay = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.ResetVelocity();

            StartCoroutine(PerformLaunch(player));
        }
    }

    private IEnumerator PerformLaunch(PlayerController player)
    {
        player.moveController.enabled = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * launchForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.1f);

        if (player.TryGetComponent(out GlideControl glider))
        {
            glider.InitiateGlide();
            yield return new WaitForSeconds(gliderDelay);
            glider.StartGliding();
        }

        if (player.TryGetComponent(out GlideControl g) && !g.isGliding)
        {
            player.moveController.enabled = true;
        }
    }
}

