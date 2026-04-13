using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrappleGunCtrl : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRender;
    public Transform barrel;
    public PlayerController playerScript; 
    public LayerMask Grappleable;
    public TextMeshProUGUI textGrappleGun;

    [Header("Settings")]
    public float maxDistance = 100f;
    public float pullSpeed = 30f;

    [Header("Crosshair Settings")]
    public Image canGrappleCrosshair;
    public Image NormalDotSite;

    private Vector3 grapplePoint;
    private bool isPullingPlayer = false;
    private bool isPullingObject = false;
    private Rigidbody targetBody;

    void Start()
    {
        if (canGrappleCrosshair != null)
        {
            canGrappleCrosshair.enabled = false;
            NormalDotSite.enabled = true;
        }
        lineRender.positionCount = 0;
    }

    void Update()
    {
        UpdateCrosshairVisibility();

        if (Input.GetMouseButtonDown(2)) StartPullPlayer();
        if (Input.GetMouseButtonUp(2)) StopGrapple();

        if (Input.GetMouseButtonDown(1)) StartPullObject();
        if (Input.GetMouseButtonUp(1)) StopGrapple();
    }

    void LateUpdate()
    {
        if (isPullingPlayer)
        {
            DrawRope(grapplePoint);

         
            Vector3 newPos = Vector3.MoveTowards(playerScript.transform.position, grapplePoint, pullSpeed * Time.deltaTime);
            playerScript.transform.position = newPos;

         
            playerScript.ResetVelocity();

            if (Vector3.Distance(playerScript.transform.position, grapplePoint) < 2f) StopGrapple();
        }

        if (isPullingObject && targetBody != null)
        {
            DrawRope(targetBody.position);
            targetBody.position = Vector3.MoveTowards(targetBody.position, barrel.position, pullSpeed * Time.deltaTime);
            targetBody.linearVelocity = Vector3.zero;

            if (Vector3.Distance(targetBody.position, barrel.position) < 1.5f) StopGrapple();
        }
    }

    void UpdateCrosshairVisibility()
    {
        if (canGrappleCrosshair == null) return;

    
        Transform camTransform = playerScript.firstPersonCam.transform;
        bool canGrapple = Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, maxDistance, Grappleable);

        canGrappleCrosshair.enabled = canGrapple;
        NormalDotSite.enabled = !canGrapple;
    }

    void StartPullPlayer()
    {
        Transform camTransform = playerScript.firstPersonCam.transform;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, maxDistance, Grappleable))
        {
            grapplePoint = hit.point;
            isPullingPlayer = true;
            lineRender.positionCount = 2;

         
            playerScript.moveController.enabled = false;
        }
    }

    void StartPullObject()
    {
        Transform camTransform = playerScript.firstPersonCam.transform;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, maxDistance, Grappleable))
        {
            if (hit.collider.TryGetComponent(out Rigidbody rb))
            {
                targetBody = rb;
                isPullingObject = true;
                lineRender.positionCount = 2;
            }
        }
    }

    public void StopGrapple()
    {
        if (isPullingPlayer)
        {
        
            playerScript.moveController.enabled = true;
        }

        isPullingPlayer = false;
        isPullingObject = false;
        targetBody = null;
        lineRender.positionCount = 0;
    }

    void DrawRope(Vector3 targetPos)
    {
        lineRender.SetPosition(0, barrel.position);
        lineRender.SetPosition(1, targetPos);
    }
}