using UnityEngine;
using UnityEngine.InputSystem;

public class GlideControl : MonoBehaviour
{
    [Header("Glide Settings")]
    public float glideGravity = -2f;
    public float glideSpeed = 10f;
    public float turnSpeed = 100f;
    public GameObject Glider; 
    public float horizontalInput = 0;
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
        if (!isGliding) ; //return;

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