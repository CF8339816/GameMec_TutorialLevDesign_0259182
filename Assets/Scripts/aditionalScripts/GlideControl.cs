using UnityEngine;
using UnityEngine.InputSystem;

public class GlideControl : MonoBehaviour
{
    [Header("Glide Settings")]
    public float glideGravity = -2f;
    public float glideSpeed = 15f;
    public float turnSpeed = 100f;
    public GameObject GliderShell;
    public float horizontalInput = 0;
    [SerializeField] float landDistance = 3f;
    [SerializeField] LayerMask groundMask;

    private PlayerController playerCtrl;
    private Rigidbody rb;
    public bool isGliding { get; private set; } = false;
    private bool canGlide = false;
    void Awake()
    {
    horizontalInput = 0;
    playerCtrl = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

    }

    void Start()
    {
        
        isGliding = false; //ensures all glide is off at start of stage
        if (GliderShell) GliderShell.SetActive(false);
    }
    public void InitiateGlide()
    {
        canGlide = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGliding)
        {
            if (collision.gameObject.CompareTag("ground"))
            {
                StopGliding();
            }
        }
    }
    public void StartGliding()
    {
        Debug.Log($"Attempting Glide: Permission={canGlide}, AlreadyGliding={isGliding}");

        if (!canGlide || isGliding)
        {
            Debug.LogWarning("Glide failed: No permission or already gliding.");
            return;
        }

        isGliding = true;

        if (GliderShell != null)
        {
            GliderShell.SetActive(true);
            Debug.Log("Glider Visual Activated!");
        }
        else
        {
            Debug.LogError("Glider Visual is NOT assigned in the Inspector!");
        }

        playerCtrl.moveController.enabled = false;
        
        //if (!canGlide || isGliding)
                                                   //{
                                                   //    return; 
                                                   //}

        //isGliding = true;
        //if (GliderShell)
        //{
        //    GliderShell.SetActive(true);
        //}

        //playerCtrl.moveController.enabled = false;
    }

    void FixedUpdate()
    {
        if (!isGliding) return;

        float horizontalInput = playerCtrl.GetInputVector().x;

        float rotation = horizontalInput * turnSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, rotation, 0);

        //if (Keyboard.current != null)
        //{
        //    if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontalInput = 1;
        //    else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontalInput = -1;
        //}
        //float rotation = Input.GetAxis("Horizontal") * turnSpeed * Time.fixedDeltaTime;
        //transform.Rotate(0, rotation, 0);

        playerCtrl.SetVerticalVelocity(glideGravity);

        Vector3 forwardMove = transform.forward * glideSpeed;
        rb.linearVelocity = new Vector3(forwardMove.x, glideGravity, forwardMove.z);

        //Vector3 glideVel = transform.forward * glideSpeed;
        //glideVel.y = glideGravity;

        //rb.linearVelocity = glideVel;

        CheckLanding();
    }

    void CheckLanding()
    {
        float sphereRadius = 0.5f; // trying spherecast to catch mor chance of touchdown

        bool hitGround = Physics.SphereCast(transform.position, sphereRadius, Vector3.down, out RaycastHit hit, landDistance, groundMask);

      
        Debug.DrawRay(transform.position, Vector3.down * landDistance, hitGround ? Color.green : Color.red);

        if (hitGround)
        {
            Debug.Log($"Glider touched ground: {hit.collider.name}");
            StopGliding();
            return;
        }

    
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f && Time.timeSinceLevelLoad > 1f)// stops glide if forward momentum is stopped 
        {
           StopGliding(); 
        }


        //bool hittingGround = Physics.Raycast(transform.position, Vector3.down, landDistance, groundMask);

        //if (hittingGround)
        //{
        //    StopGliding();
        //}
    }

    public void StopGliding()
    {
        if (!isGliding)
        {
            return;
        }

        isGliding = false;
        canGlide = false;

        if (GliderShell)
        {
            GliderShell.SetActive(false);
        }

        rb.linearVelocity = Vector3.zero;
        if (playerCtrl != null && playerCtrl.moveController != null)
        {
            playerCtrl.moveController.enabled = true;
        }
        //playerCtrl.moveController.enabled = true;
    }
}