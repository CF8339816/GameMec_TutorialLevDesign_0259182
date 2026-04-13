using UnityEngine;
using System.Collections;

public class SpringLauncher : MonoBehaviour
{
    [Header("Launch Settings")]
    public float launchForce = 25f; 
    public float gliderDelay = 1.5f;

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out PlayerController player))
        {
         
            player.ResetVelocity();

           
            player.SetVerticalVelocity(launchForce);

            if (other.TryGetComponent(out GlideControl glider))
            {
              
                glider.StopGliding();
                StartCoroutine(ActivateGliderAfterDelay(glider));
            }
        }
    }

    private IEnumerator ActivateGliderAfterDelay(GlideControl glider)
    {
        yield return new WaitForSeconds(gliderDelay);

       
        glider.StartGliding();
    }
}

