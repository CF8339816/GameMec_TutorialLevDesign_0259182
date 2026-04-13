using UnityEngine;

public class GlideControl : MonoBehaviour
{
    [Header("Glide Settings")]
    public float glideGravity = -2f;
    public float glideSpeed = 10f;
    public float turnSpeed = 100f;
    public GameObject Glider; 

    [SerializeField] float landDistance = 0.8f;
    [SerializeField] LayerMask groundMask;

    private PlayerController playerCtrl;
    private Rigidbody rb;
    public bool isGliding { get; private set; } = false;

    void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartGliding()
    {
        if (isGliding) return;

        isGliding = true;
        if (Glider) Glider.SetActive(true);

        
        playerCtrl.moveController.enabled = false;
    }

    void FixedUpdate()
    {
        if (!isGliding) return;

       
        float rotation = Input.GetAxis("Horizontal") * turnSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, rotation, 0);

      
        Vector3 glideVel = transform.forward * glideSpeed;
        glideVel.y = glideGravity;

        rb.linearVelocity = glideVel;

        CheckLanding();
    }

    void CheckLanding()
    {
        
        bool hittingGround = Physics.Raycast(transform.position, Vector3.down, landDistance, groundMask);

        if (hittingGround)
        {
            StopGliding();
        }
    }

    public void StopGliding()
    {
        if (!isGliding) return;

        isGliding = false;
        if (Glider) Glider.SetActive(false);

        
        playerCtrl.moveController.enabled = true;
    }
}