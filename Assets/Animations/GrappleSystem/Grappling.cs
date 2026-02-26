using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
   // private PlayerMovementGrappling pm;
    public Transform cam;
    public Transform lineSquirter;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling & Pulling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    public float pullSpeed = 10f; // Speed at which objects are pulled

    private Vector3 grapplePoint;
    private Rigidbody pulledObject; // Stores the object currently being pulled

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
    //    pm = GetComponent<PlayerMovementGrappling>();
    }

    private void Update()
    {
        // Start grapple or pull on initial press
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

        // Release object when key is let go
        if (Input.GetKeyUp(grappleKey)) StopGrapple();

        // While held, pull the object toward the player
        if (Input.GetKey(grappleKey) && pulledObject != null)
        {
            PullObject();
        }

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (grappling || pulledObject != null)
        {
            lr.enabled = true;
            lr.SetPosition(0, lineSquirter.position);
            lr.SetPosition(1, pulledObject != null ? pulledObject.position : grapplePoint);
        }
        else
        {
            lr.enabled = false;
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            // Check if hit object can be pulled (has a Rigidbody)
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                pulledObject = rb;
            }
            else
            {
                // Standard grappling logic if not a pullable physics object
                grappling = true;
               // pm.freeze = true;
                grapplePoint = hit.point;
                Invoke(nameof(ExecuteGrapple), grappleDelayTime);
            }
        }
    }

    private void PullObject()
    {
        // Calculate direction toward player's gun tip or center
        Vector3 direction = (lineSquirter.position - pulledObject.position).normalized;
        float distance = Vector3.Distance(lineSquirter.position, pulledObject.position);

        // Stop pulling if it gets too close
        if (distance > 1.5f)
        {
            pulledObject.velocity = direction * pullSpeed;
        }
        else
        {
            pulledObject.velocity = Vector3.zero;
        }
    }

    private void ExecuteGrapple()
    {
       // pm.freeze = false;
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;
        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

      //  pm.JumpToPosition(grapplePoint, highestPointOnArc);
    }

    public void StopGrapple()
    {
      //  pm.freeze = false;
        grappling = false;
        pulledObject = null; // Clear the reference to release the object
        grapplingCdTimer = grapplingCd;
    }
}