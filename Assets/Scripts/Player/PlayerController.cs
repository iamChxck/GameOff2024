using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInputActions inputActions; // Reference to the generated class

    #region Input Actions
    private InputAction movementAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction zoomAction;
    private InputAction crouchAction;
    #endregion

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = .25f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Vector2 lookInput;
    private Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    [HideInInspector] // Don't really need to see this on the inspector
    public bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    private bool isWalking = false;

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    #region Jump

    public bool enableJump = true;
    public float jumpPower = 5f;

    // Internal Variables
    private bool isGrounded = false;

    #endregion

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        crosshairObject = GetComponentInChildren<Image>();

        // Set internal variables
        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }

        // Instantiate input action singleton
        inputActions = InputActionSingleton.Instance;

        // Access the input actions
        movementAction = inputActions.Player.Movement;
        lookAction = inputActions.Player.Look;
        jumpAction = inputActions.Player.Jump;
        zoomAction = inputActions.Player.Zoom;
        crouchAction = inputActions.Player.Crouch;

        // Bind the actions to a method
        jumpAction.performed += ctx => Jump();
        zoomAction.started += ctx => OnZoomStarted();
        zoomAction.canceled += ctx => OnZoomCanceled();
        crouchAction.started += ctx => OnCrouchStarted();
        crouchAction.canceled += ctx => OnCrouchCanceled();
    }


    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar

        sprintBarCG = GetComponentInChildren<CanvasGroup>();

        if (useSprintBar)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            sprintBarWidth = screenWidth * sprintBarWidthPercent;
            sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull)
            {
                sprintBarCG.alpha = 0;
            }
        }
        else
        {
            sprintBarBG.gameObject.SetActive(false);
            sprintBar.gameObject.SetActive(false);
        }

        #endregion
    }

    private void Update()
    {
        #region Camera

        HandleCameraMovement();

        #region Camera Zoom
        if (enableZoom)
        {
            // Lerps camera.fieldOfView to allow for a smooth transition
            if (isZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else if (!isZoomed && !isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
            }
        }
        #endregion
        #endregion

        #region Sprint

        if (enableSprint)
        {
            //SEPERATE INTO FUNCTION
            if (isSprinting)
            {
                isZoomed = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                // Drain sprint remaining while sprinting
                if (!unlimitedSprint)
                {
                    sprintRemaining -= 1 * Time.deltaTime;
                    if (sprintRemaining <= 0)
                    {
                        isSprinting = false;
                        isSprintCooldown = true;
                    }
                }
                //return;
            }

            else
            {
                // Regain sprint while not sprinting
                sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
            }

            // Handles sprint cooldown 
            // When sprint remaining == 0 stops sprint ability until hitting cooldown

            //SEPERATE INTO FUNCTION
            if (isSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                {
                    isSprintCooldown = false;
                }
                //return;
            }
            else
            {
                sprintCooldown = sprintCooldownReset;
            }

            // Handles sprintBar 
            if (useSprintBar && !unlimitedSprint)
            {
                float sprintRemainingPercent = sprintRemaining / sprintDuration;
                sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
            }
        }

        #endregion

        CheckGround();

        if (enableHeadBob)
        {
            HeadBob();
        }
    }

    private void HandleCameraMovement()
    {
        if (!cameraCanMove) return;

        lookInput = lookAction.ReadValue<Vector2>();
        yaw += lookInput.x * mouseSensitivity;
        pitch = Mathf.Clamp(pitch + (invertCamera ? lookInput.y : -lookInput.y) * mouseSensitivity, -maxLookAngle, maxLookAngle);

        ApplyCameraRotation();
    }

    private void ApplyCameraRotation()
    {
        transform.localEulerAngles = new Vector3(0, yaw, 0);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }


    // Sets isGrounded based on a raycast sent straigth down from the player object
    private void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.down * (transform.localScale.y * 0.5f);
        isGrounded = Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.75f);
        Debug.DrawRay(origin, Vector3.down * 0.75f, isGrounded ? Color.green : Color.red);
    }


    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (isGrounded)
        {
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
        }

        // When crouched and using toggle system, will uncrouch for a jump
        if (isCrouched && !holdToCrouch)
        {
            Crouch();
        }
    }

    #region Crouch Methods
    private void Crouch()
    {
        transform.localScale = isCrouched ? originalScale : new Vector3(originalScale.x, crouchHeight, originalScale.z);
        walkSpeed = isCrouched ? walkSpeed / speedReduction : walkSpeed * speedReduction;
        isCrouched = !isCrouched;
    }

    private void OnCrouchStarted()
    {
        if (enableCrouch)
        {
            if (holdToCrouch)
            {
                if (!isCrouched) Crouch();
                return;
            }
            Crouch();
        }
    }

    private void OnCrouchCanceled()
    {
        if (enableCrouch && holdToCrouch && isCrouched)
        {
            Crouch();
        }
    }


    #endregion

    #region HeadBob
    private void HeadBob()
    {
        timer += Time.deltaTime * GetBobSpeed();
        joint.localPosition = isWalking ? GetBobPosition() : ResetBobPosition();
    }

    private float GetBobSpeed()
    {
        if (isSprinting) return bobSpeed + sprintSpeed;
        if (isCrouched) return bobSpeed * speedReduction;
        return bobSpeed;
    }

    private Vector3 GetBobPosition()
    {
        return new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x,
                           jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y,
                           jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
    }

    private Vector3 ResetBobPosition()
    {
        timer = 0;
        return Vector3.Lerp(joint.localPosition, jointOriginalPos, Time.deltaTime * bobSpeed);
    }
    #endregion

    #region Zoom Methods
    private void OnZoomStarted()
    {
        if (enableZoom && !isSprinting)
        {
            isZoomed = holdToZoom ? true : !isZoomed;
        }
    }

    private void OnZoomCanceled()
    {
        if (enableZoom && holdToZoom) isZoomed = false;
    }

    #endregion

    private void FixedUpdate()
    {
        if (!playerCanMove) return;

        Vector2 inputVector = movementAction.ReadValue<Vector2>();
        Vector3 targetVelocity = new Vector3(inputVector.x, 0, inputVector.y);
        targetVelocity = HandleSprintingMovement(targetVelocity);

        HandleMovement(targetVelocity);
    }

    #region Sprinting
    private Vector3 HandleSprintingMovement(Vector3 velocity)
    {
        if (enableSprint && Keyboard.current.leftShiftKey.isPressed && sprintRemaining > 0f && !isSprintCooldown)
        {
            velocity = transform.TransformDirection(velocity) * sprintSpeed;
            isSprinting = true;
            if (isCrouched) Crouch();
            sprintBarCG.alpha += 5 * Time.deltaTime;
        }
        else
        {
            velocity = transform.TransformDirection(velocity) * walkSpeed;
            isSprinting = false;
            if (sprintBarCG.alpha > 0 && hideBarWhenFull) sprintBarCG.alpha -= 3 * Time.deltaTime;
        }
        return velocity;
    }

    private void HandleSprinting()
    {
        if (isSprinting)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);
            UpdateSprintRemaining(-Time.deltaTime);
        }
        else
        {
            UpdateSprintRemaining(Time.deltaTime);
        }

        UpdateSprintCooldown();
        UpdateSprintBar();
    }

    private void UpdateSprintRemaining(float value)
    {
        if (unlimitedSprint) return;
        sprintRemaining = Mathf.Clamp(sprintRemaining + value, 0, sprintDuration);
    }

    private void UpdateSprintCooldown()
    {
        if (isSprintCooldown)
        {
            sprintCooldown -= Time.deltaTime;
            if (sprintCooldown <= 0) isSprintCooldown = false;
        }
        else
        {
            sprintCooldown = sprintCooldownReset;
        }
    }

    private void UpdateSprintBar()
    {
        if (useSprintBar && !unlimitedSprint)
        {
            float sprintPercent = sprintRemaining / sprintDuration;
            sprintBar.transform.localScale = new Vector3(sprintPercent, 1f, 1f);
            sprintBarCG.alpha = hideBarWhenFull && sprintRemaining == sprintDuration ? 0 : 1;
        }
    }

    #endregion

    private void HandleMovement(Vector3 targetVelocity)
    {
        Vector3 velocityChange = CalculateVelocityChange(targetVelocity);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private Vector3 CalculateVelocityChange(Vector3 targetVelocity)
    {
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - velocity;
        return new Vector3(Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange),
                           0,
                           Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange)
                           );
    }


    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}


