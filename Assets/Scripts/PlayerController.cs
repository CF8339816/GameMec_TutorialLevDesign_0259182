using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input, animation, and feedback effects for character movement.
/// Works in conjunction with AdvancedMoveController to provide a complete character control system.
/// </summary>
[RequireComponent(typeof(AdvancedMoveController), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    /// <summary>
    //my added variables
    /////

   public Camera firstPersonCam;
 //
    [SerializeField] LayerMask groundMask;
    [SerializeField] float gravity = -9.8f; //has to be neg because is downward force
    public Vector3 externalVelocity;
    private Vector3 velocity;
    [SerializeField] public Transform ActiveCheckPoint;
    public AdvancedMoveController moveController;

   private GlideControl glideControl;

    /// </summary>


    public ThirdPersonCamera CameraFollower {get; private set;}
    private Animator characterAnimator;
 
    private Rigidbody rb;
    private DashController dashController;
    
    // Movement state
    private Vector3 moveDirection;
    private Vector3 cameraAlignedForward;
    private Vector3 cameraAlignedRight;
    private Vector3 inputVector;

    private HealthController healthComponent;
    private PlayerInput playerInput;
    
    public bool JoinedThroughGameManager { get; set; } = false;
    public static List<PlayerController> players = new List<PlayerController>();


    [Header("Grapple Targeting")]
    public float mouseSensitivity = 2.0f;
    private float verticalRotation = 0f;
    private bool isGrappleMode = false;
    public UnityEngine.UI.Image dotSite;
    public TextMeshProUGUI grapStatTxt;
    public GameObject Grappler;
    /// <summary>
    // My added methods
    public void SetVerticalVelocity(float y)
    {
        //velocity.y = y;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, y, rb.linearVelocity.z);
    }

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // velocity = Vector3.zero;
    }

    public Vector2 GetInputVector()
    {
        return inputVector;
    }

    /// </summary>

    private void OnEnable()
    {
        if(moveController != null)
            moveController.enabled = true;
    }

    private void OnDisable()
    {
        if (moveController != null)
            {
                inputVector = Vector3.zero;
                moveDirection = Vector3.zero;
                rb.linearVelocity = Vector3.zero;
                moveController.ApplyMovement(Vector3.zero);
                moveController.UpdateMovement();
                moveController.enabled=false;
                UpdateVisualFeedback();
            }
    }

    /// <summary>
    /// Initialize components and verify required setup
    /// </summary>
    void Awake()
    {

        players.Add(this);
        // Ensure correct tag for player identification
        if(!gameObject.CompareTag("Player"))
            tag = "Player";


        // Cache component references
         rb = GetComponent<Rigidbody>(); 
        moveController = GetComponent<AdvancedMoveController>();
   
        CameraFollower = GetComponentInChildren<ThirdPersonCamera>();
        characterAnimator = GetComponentInChildren<Animator>();
        healthComponent = GetComponent<HealthController>();

        TryGetComponent(out playerInput);
        TryGetComponent(out dashController);
 
        glideControl = GetComponent<GlideControl>();////////////
        if (firstPersonCam)
        {
            firstPersonCam.enabled = false;
        }
        if (CameraFollower)
        {
            //if (playerInput.camera == null) 
            //{
                if (playerInput != null)
                {
                    playerInput.camera = CameraFollower.GetComponent<Camera>();
                }
                CameraFollower.transform.SetParent(null);
                DontDestroyOnLoad(CameraFollower.gameObject);

                ////Debug.Log(actions["Jump"].GetBindingDisplayString());
                //playerInput.camera = CameraFollower.GetComponent<Camera>();
            //}
            //CameraFollower.transform.SetParent(transform.parent);
            //DontDestroyOnLoad(CameraFollower.gameObject);
        }

        DontDestroyOnLoad(gameObject);

      

      if (firstPersonCam) firstPersonCam.enabled = false;
       // if (grappleCamera) grappleCamera.enabled = false;
        //if (playerInput != null && firstPersonCam != null)
        //{
        //    playerInput.camera = firstPersonCam;
        //}
        /////////////////
    }

    public void Start()
    {
        if (!JoinedThroughGameManager)
        {
            Destroy(gameObject);
            return;
        }
        if (glideControl)// ensures glide is turned off
        {
            glideControl.StopGliding();
        }

        CheckpointManager.TeleportPlayerToCheckpoint(gameObject);

        if (grapStatTxt != null)
        {
            grapStatTxt.text = "GRAPPLE MODE: Out of Order";
        }

        ResetVelocity(); // to prevent freefall hanf at spawn
        moveController.enabled = true;

        Physics.SyncTransforms();// forces move ctlr to see floor

    }

    /// <summary>
    /// Clean up camera follower on destruction
    /// </summary>
    void OnDestroy()
    {
        if (players.Contains(this))
            players.Remove(this);
        if (playerInput)
            Destroy(playerInput);
        if (CameraFollower)
            Destroy(CameraFollower.gameObject);
    }

    void OnMove(InputValue inputVal)
    {
        if (GameManager.Instance.IsShowingPauseMenu)
            inputVector = Vector3.zero;
        else
            inputVector = inputVal.Get<Vector2>();
    }
    /// <summary>
    /// Handle jump input from the input system
    /// </summary>
    void OnJump()
    {
        if (!GameManager.Instance.IsShowingPauseMenu)
            moveController.RequestJump();

    }

    void OnPause()
    {
        GameManager.Instance.TogglePauseMenu();
    }

    /// <summary>
    /// Handle dash input from the input system
    /// </summary>
    void OnDash()
    {
        if (!GameManager.Instance.IsShowingPauseMenu && dashController)
            dashController.TryStartDash(moveDirection);
    }

    void OnCameraOrbit(InputValue inputVal)
    {
        if (CameraFollower)
        {
            CameraFollower.OrbitInput = inputVal.Get<float>();
        }
    }

    /// <summary>
    /// Calculate movement direction based on camera orientation
    /// </summary>
    void Update()
    {
        if (CameraFollower == null)
        {
            return;
        }
        // Convert input to camera-relative movement direction
        Quaternion cameraRotation = Quaternion.Euler(0, CameraFollower.transform.eulerAngles.y, 0);
        cameraAlignedForward = cameraRotation * Vector3.forward;
        cameraAlignedRight = cameraRotation * Vector3.right;
        
        moveDirection = ((cameraAlignedForward * inputVector.y) + (cameraAlignedRight * inputVector.x)).normalized;

        //if (Keyboard.current.gKey.wasPressedThisFrame)
        //{
        //    ToggleGrappleMode();
        //}

        //if (isGrappleMode)
        //{
        //    HandleFirstPersonAiming();
        //}
        //else
        //{
        //    HandleThirdPersonMovement();
        //}
    }


    //private void ToggleGrappleMode()
    //{
    //    if (Grappler != null)
    //    {
    //        Grappler.SetActive(isGrappleMode);
    //    }

    //    isGrappleMode = !isGrappleMode;

    //    if (firstPersonCam != null)
    //    {
    //        firstPersonCam.enabled = isGrappleMode;

    //        if (firstPersonCam.TryGetComponent(out AudioListener fpListener))
    //        {
    //            fpListener.enabled = isGrappleMode;
    //        }
    //    }   
        
        
    //    if (CameraFollower != null)
    //    {
    //        CameraFollower.GetComponent<Camera>().enabled = !isGrappleMode;
    //        //CameraFollower.gameObject.SetActive(!isGrappleMode);

    //        if (CameraFollower.TryGetComponent(out AudioListener tpListener))
    //        {
    //            tpListener.enabled = !isGrappleMode;
    //        }
    //    }
            
    //        if (dotSite != null)
    //    {
    //        dotSite.enabled = isGrappleMode;
    //    }

    //    if (grapStatTxt != null)
    //    {
    //        grapStatTxt.text = isGrappleMode ? "GRAPPLE MODE: ON" : "GRAPPLE MODE: OFF";

    //    }

    //    Cursor.lockState = isGrappleMode ? CursorLockMode.Locked : CursorLockMode.None;
    //    Cursor.visible = !isGrappleMode;
    //}

    //private void HandleFirstPersonAiming()
    //{
    //    Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * 0.1f;

    //    transform.Rotate(Vector3.up * mouseDelta.x);

    //    verticalRotation -= mouseDelta.y;
    //    verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f); 
    //    if (firstPersonCam)
    //    {
    //        firstPersonCam.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    //    }

    //    moveDirection = Vector3.zero;
    //}

    //private void HandleThirdPersonMovement()
    //{
    //    if (CameraFollower == null)
    //    {
    //        return;
    //    }
    //    Quaternion cameraRotation = Quaternion.Euler(0, CameraFollower.transform.eulerAngles.y, 0);
    //    Vector3 forward = cameraRotation * Vector3.forward;
    //    Vector3 right = cameraRotation * Vector3.right;

    //    moveDirection = ((forward * inputVector.y) + (right * inputVector.x)).normalized;
    //}

    /// <summary>
    /// Handle physics-based movement and animation updates
    /// </summary>
    void FixedUpdate()
    {
        if (moveController.enabled) 
        {
            moveController.ApplyMovement(moveDirection);
            moveController.UpdateMovement();
        }

        // Normal movement
        UpdateVisualFeedback();

        if (transform.position.y < -1000f)
        {
            CheckpointManager.TeleportPlayerToCheckpoint(gameObject);
            ResetVelocity();

            if (CameraFollower)
            {
                CameraFollower.transform.position = gameObject.transform.position;
            }
        }
    }

    /// <summary>
    /// Update animator parameters and handle squash/stretch effects
    /// </summary>
    private void UpdateVisualFeedback()
    {
        if (!characterAnimator) return;


        // Update animator parameters
        //characterAnimator.SetFloat(MovementController.AnimationID_DistanceToTarget, moveController.distanceToDestination);
        characterAnimator.SetBool(MovementController.AnimationID_IsGrounded, moveController.isGrounded);
        characterAnimator.SetFloat(MovementController.AnimationID_YVelocity, rb.linearVelocity.y);
    }

} 

