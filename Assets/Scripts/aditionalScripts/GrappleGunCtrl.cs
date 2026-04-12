using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrappleGunCtrl : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRender;
    public Transform barrel;
    public Transform firstPersonCam;
    public Transform character;
    public PlayerController playerScript;
    public LayerMask Grappleable;
    public TextMeshProUGUI textGrappleGun;
    [Header("Settings")]
    public float maxDistance = 100f;
    public float pullSpeed = 20f;

    [Header("Crosshair Settings")]
    public Image crosshair;
    //public Color normalColor = Color.white;
    //public Color canGrappleColor = Color.green;

    private Vector3 grapplePoint;
    private SpringJoint joint;
    private bool isPullingPlayer = false;
    private bool isPullingObject = false;
    private Rigidbody targetBody;

    void Start()
    {

        if (crosshair != null)
            crosshair.enabled = false;


    }
    void Update()
    {
        //    if (playerScript == null || !playerScript.PowerOn)
        //    {
        //        if (crosshair != null) crosshair.enabled = false;
        //        if (isPulling || joint != null) StopGrapple();
        //        return;
        //    }
        UpdateCrosshairVisibility();

        //if (Input.GetMouseButtonDown(2)) StartPull();//MiddleMouse
        if (Input.GetMouseButtonDown(2)) StartPullPlayer();
        if (Input.GetMouseButtonUp(2)) StopGrapple();

        //if (Input.GetMouseButtonDown(1)) StartSwing();//rightMouse
        if (Input.GetMouseButtonDown(1)) StartPullObject();
        if (Input.GetMouseButtonUp(1)) StopGrapple();
    }

    void LateUpdate()
    {
        //DrawRope();
        //if (isPullingPlayer)
        //{
        //   character.position = Vector3.MoveTowards(character.position, grapplePoint, pullSpeed * Time.deltaTime);
        //}

        if (isPullingPlayer)
        {
            DrawRope(grapplePoint);
            character.position = Vector3.MoveTowards(character.position, grapplePoint, pullSpeed * Time.deltaTime);
            if (Vector3.Distance(character.position, grapplePoint) < 1.5f) StopGrapple();
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
        //if (crosshair == null ) return;
        if (crosshair == null || !playerScript.PowerOn) return;
        {


            //if (playerScript.PowerOn == true)
            //{

            bool canGrapple = Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable);

            crosshair.enabled = canGrapple;
            //}
        }
    }
    void StartPullPlayer()
    {
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
        {
            grapplePoint = hit.point;
            isPullingPlayer = true;
            lineRender.positionCount = 2;
        }
    }
    void StartPullObject()
    {
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
        {

            if (hit.collider.GetComponent<Rigidbody>())
            {
                targetBody = hit.collider.GetComponent<Rigidbody>();
                isPullingObject = true;
                lineRender.positionCount = 2;
            }
        }
    }
    //void StartSwing()
    //{
    //    if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
    //    {
    //        grapplePoint = hit.point;
    //        joint = character.gameObject.AddComponent<SpringJoint>();
    //        joint.autoConfigureConnectedAnchor = false;
    //        joint.connectedAnchor = grapplePoint;

    //        float distanceFromPoint = Vector3.Distance(character.position, grapplePoint);
    //        joint.maxDistance = distanceFromPoint * 0.8f;
    //        joint.minDistance = distanceFromPoint * 0.25f;


    //        joint.spring = 4.5f;
    //        joint.damper = 7f;
    //        joint.massScale = 4.5f;

    //        lineRender.positionCount = 2;
    //    }
    //}

    void StopGrapple()
    {
        lineRender.positionCount = 0;
        isPullingPlayer = false;
        isPullingObject = false;
        targetBody = null;
        lineRender.positionCount = 0;
        //isPulling = false;
        //Destroy(joint);
    }


    void DrawRope(Vector3 targetPos)
    {
        lineRender.SetPosition(0, barrel.position);
        lineRender.SetPosition(1, targetPos);
    }
    //void DrawRope()
    //{
    //    if (!joint && !isPulling) return;
    //    lineRender.SetPosition(0, barrel.position);
    //    lineRender.SetPosition(1, grapplePoint);
    //}
}
