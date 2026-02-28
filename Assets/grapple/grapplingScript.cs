using System.Collections;
using UnityEngine;

public class PullGrapple : MonoBehaviour
{
    [Header("References")]
    private PlayerController pm;
    public Transform cam;
    public Transform grapplerTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    public GameObject Grappler;
   // public GameObject Cam;

    [Header("Pull Settings")]
    public float maxGrappleDistance = 50f;
    public float pullSpeed = 20f;
    public float stopDistance = 2f; // Distance from player to stop pulling
    public float grapplingCd = 1f;

    private GameObject pulledObject;
    private Rigidbody targetRb;
    private float grapplingCdTimer;
    private bool isPulling;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;



    private void Start()
    {
      //  lr = GetComponent<LineRenderer>();
        pm = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey) && grapplingCdTimer <= 0)
        {
            Grappler.SetActive(true);
            StartGrapple();
            Debug.Log("<color=green>You Whipped it out!</color>Time to grapple....");

        }
        if (isPulling)
        {
            ExecutePull();
            Debug.Log("<color=blue>Yank that cable back!</color>Bring it on back....");
        }
        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (isPulling && pulledObject != null)
        {
            lr.enabled = true;
            lr.SetPosition(0, grapplerTip.position);
            lr.SetPosition(1, pulledObject.transform.position);
        }
        else
        {
            lr.enabled = false;
        }
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            pulledObject = hit.collider.gameObject;
            targetRb = pulledObject.GetComponent<Rigidbody>(); //  targets the raycast to the objects ridgid body
            Debug.Log("<color=green> We have connection !</color>Fuirst contact....");

            if (targetRb != null)// prevenmts pulling objects with no ridgid body
            {
                isPulling = true;
                targetRb.useGravity = false; // Disable gravity while pulling
                targetRb.linearDamping = 0;   // Prevent air resistance from slowing it down
            }
        }
    }

    private void ExecutePull()
    {
        if (pulledObject == null)
        {
            Grappler.SetActive(false);
            StopGrapple();
            return;
        }

        float distance = Vector3.Distance(grapplerTip.position, pulledObject.transform.position);

        if (distance > stopDistance)
        {
           
            Vector3 direction = (grapplerTip.position - pulledObject.transform.position).normalized; //sets direction of pull toward player

           
            targetRb.linearVelocity = direction * pullSpeed; //applies velocity to the pull
        }
        else
        {
            StopGrapple();
        }
    }

    public void StopGrapple()
    {
        isPulling = false;

        if (targetRb != null)
        {
            targetRb.useGravity = true;
            targetRb.linearVelocity = Vector3.zero; // Stop the object when it gets to you
        }

        pulledObject = null;
        targetRb = null;
        grapplingCdTimer = grapplingCd;
    }
}