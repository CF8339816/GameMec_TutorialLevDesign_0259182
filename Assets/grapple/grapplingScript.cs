using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class grapplingScript : MonoBehaviour
{
  
    [Header("References")]
    private PlayerMovementGrappling pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;


    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;


    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        pm = GetComponent<PlayerMovementGrappling>();
    }



    private void StartGrapple()
    {

    }

    private void ExecuteGrapple()
    {

    }

    public void StopGrapple()
    {

    }


}
}
}
