using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    Vector2 playerInput;
    float moveSpeed;
    public float walkSpeed, runSpeed;
    GameObject playerCharacter;
    bool isRunningKey, isRunningStick, isRunning;
    bool isGrounded;
    public float groundCheckDistance = 0.01f;
    public LayerMask groundLayer;
    Animator anim;


    [Header("Camera parameters")]
    Transform mainCamera;
    Transform camTransform;
    public Vector3 distance;
    [SerializeField] Transform lookAt;
    [SerializeField] Transform playerOrientation;
    public float turnSpeed = 2.0f;
    bool canSwipe;
    Vector2 TouchScreenInitialPos, TouchScreenTemporaryPos, TouchScreenCurrentPos;
    Vector2 direction;
    float dir;
    Vector3 offset;
    public float cameraTurnSpeed = 4.0f;
    Vector3 lookAtInitialPositioning;
    public float minHeight, maxHeight;
    Vector3 offsetY;
    Vector2 mousePos, mouseTempPos;
    public bool isPC, isConsole, isHandheld;
    Vector2 rightStickVector;

    [Header("Jump")]
    bool jump, jumped;
    public float gravity = 9.0f;
    public float gravityMultiplier = 1f;
    public float jumpForce = 50f;
    [Header("Climb parameters")]
    bool climb, canClimb;
    bool hangToClimb, normalClimb, jumpOverClimb;
    bool bottomRayHit, middleRayHit, topRayHit, verticalRayHit, hangRayHit;
    float posX, posY, posZ, horizontalHitPos;
    public float horizontalRange, verticalRange;
    public Vector3 bottomOffset, middleOffset, topOffset, hangOffset, verticalOffset;
    CapsuleCollider capCollider;
    Vector3 moveVector;
    bool animating;



    Rigidbody rb;

    [Header("Crouch parameters")]
    bool crouch, isMoving, canChange, running, sliding;
    public bool canSlide;
    public float slideForce;
    public float crouchSpeed= 4.0f;
    float capsuleHeight;

    


    private void Start()
    {
        anim = GetComponent<Animator>();
        playerCharacter = GameObject.FindWithTag("Player");
        mainCamera = GameObject.FindWithTag("MainCamera").transform;
        camTransform = GameObject.FindWithTag("camTransform").transform;
        rb = GetComponent<Rigidbody>();
        lookAtInitialPositioning = lookAt.localPosition;
        offset = distance;
        capCollider = GetComponent<CapsuleCollider>();
        
        capsuleHeight = capCollider.height;
        
        canChange = true;

    }

    public void Move(InputAction.CallbackContext _context)
    {
        playerInput = _context.ReadValue<Vector2>();
    }
    public void Running(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            isRunningKey = true;
        }
        if (_context.performed)
        {
            isRunningKey = true;
        }
        if (_context.canceled)
        {
            isRunningKey = false;
        }
    }

    public void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);
        MovePlayer();

        if (playerInput != Vector2.zero)
        {
            moveSpeed = walkSpeed;
        }
        if (isRunningKey || isRunningStick)
        {
            isRunning = true;
        }
        else if (!isRunningKey && !isRunningStick)
        {
            isRunning = false;
        }
        if (isHandheld)
        {
       /*     TouchScreenRotation();
        }
        if (isPC)
        {*/
    /*        MouseRotation();
        }
        if (isConsole)
        {*/
            RightStickRotation();
        }
        if (!isGrounded)
        {
            rb.AddForce(-gravityMultiplier * Vector3.up, ForceMode.Acceleration);
        }

        if (isGrounded && jumped)
        {
            jump = false;
            jumped = false;
            climb = false;
        }
        if (climb && playerInput.y < 0)
        {
            anim.SetBool("drop", true);
        }
        if (climb && playerInput.y >= 0)
        {
            anim.SetBool("drop", false);
        }
        if (!isGrounded && !jump && !climb)
        {
            anim.SetTrigger("falling");
            anim.SetBool("walk", false);
            anim.SetBool("run", false);
            anim.SetBool("idle", false);

        }

        if(isGrounded && moveVector != Vector3.zero)
        {
            isMoving = true;
        }
        if (isGrounded && moveVector == Vector3.zero)
        {
            isMoving = false;
        }
        if (isMoving && crouch)
        {
            anim.SetTrigger("crouchWalk");
        }
        if (!isMoving && crouch)
        {
            anim.SetTrigger("walkToCrouch");
        }
        if (isMoving && !crouch && !canChange && !sliding)
        {
            anim.SetTrigger("walkToCrouch");
            anim.SetBool("idle", true);
        }
        if (sliding && !crouch)
        {
            canChange = true;
            sliding = false;
            animating = false;
            rb.velocity = Vector3.zero;
            capCollider.height = capsuleHeight;
            capCollider.center = new Vector3(capCollider.center.x, capsuleHeight / 2f, capCollider.center.z);
        }
        if (!sliding && !crouch)
        {
            capCollider.height = capsuleHeight;
            capCollider.center = new Vector3(capCollider.center.x, capsuleHeight / 2f, capCollider.center.z);
        }

        
       RayCasting();
    }
    void RayCasting()
    {
        Vector3 horizontalDirection = Vector3.forward;
        Vector3 verticalDirection = Vector3.up;
        Ray rayBottom = new Ray(transform.position + bottomOffset, transform.TransformDirection(horizontalDirection * horizontalRange));
        Ray rayMiddle = new Ray(transform.position + middleOffset, transform.TransformDirection(horizontalDirection * horizontalRange));
        Ray rayTop = new Ray(transform.position + topOffset, transform.TransformDirection(horizontalDirection * horizontalRange));
        Ray rayHang = new Ray(transform.position + hangOffset, transform.TransformDirection(horizontalDirection * horizontalRange));
        Ray rayVertical = new Ray(transform.position + verticalOffset + (transform.forward * 0.6f), 
            transform.TransformDirection(-verticalDirection * verticalRange));

        bottomRayHit = Physics.Raycast(rayBottom, out RaycastHit hitBottom, horizontalRange);
        middleRayHit = Physics.Raycast(rayMiddle, out RaycastHit hitMiddle, horizontalRange); ;
        topRayHit = Physics.Raycast(rayTop, out RaycastHit hitTop, horizontalRange);
        hangRayHit = Physics.Raycast(rayHang, out RaycastHit hitHang, horizontalRange);
        verticalRayHit = Physics.Raycast(rayVertical, out RaycastHit hitVertical, verticalRange);

        if (bottomRayHit && !middleRayHit && !hangRayHit && !topRayHit)
        {
            canClimb = false;
        }
        if (!bottomRayHit && !middleRayHit && !hangRayHit && !topRayHit)
        {
            canClimb = false;
        }
        if(topRayHit && middleRayHit && hangRayHit)
        {
            canClimb = false;
        }
        if(middleRayHit && !topRayHit && !hangRayHit) 
        {
            canClimb = true;
            normalClimb = true;
            jumpOverClimb = false;
            hangToClimb = false;
            horizontalHitPos = hitMiddle.point.y;
        }
        if (topRayHit && middleRayHit && !hangRayHit)
        {
            canClimb = true;
            hangToClimb = true;
            jumpOverClimb = false;
            normalClimb = false;
        }
        if (bottomRayHit && !middleRayHit && hangRayHit)
        {
            canClimb = true;
            jumpOverClimb = true;
            hangToClimb = false;
            normalClimb = false;
        }
        if (bottomRayHit && !middleRayHit && topRayHit && !hangRayHit)
        {
            canClimb = true;
            hangToClimb = true;
            jumpOverClimb = false;
            normalClimb = false;
        }
        posY = hitVertical.point.y;



        Debug.DrawRay(transform.position + bottomOffset, transform.TransformDirection(horizontalDirection * horizontalRange));
        Debug.DrawRay(transform.position + middleOffset, transform.TransformDirection(horizontalDirection * horizontalRange));
        Debug.DrawRay(transform.position + topOffset, transform.TransformDirection(horizontalDirection * horizontalRange));
        Debug.DrawRay(transform.position + hangOffset, transform.TransformDirection(horizontalDirection * horizontalRange));
        Debug.DrawRay(transform.position + verticalOffset + (transform.forward * 0.6f),
            transform.TransformDirection(-verticalDirection * verticalRange));
    } 
    private void LateUpdate()
    {
        offset = Quaternion.AngleAxis(dir * cameraTurnSpeed, Vector3.up) * offset;
        mainCamera.position = transform.position + offset;
        camTransform.position = transform.position + offset;
        
        mainCamera.LookAt(lookAt.position);
        camTransform.LookAt(new Vector3(lookAt.position.x, camTransform.position.y, lookAt.position.z));

        lookAt.localPosition = new Vector3(lookAt.localPosition.x, Mathf.Clamp(lookAt.localPosition.y, lookAtInitialPositioning.y - minHeight, lookAtInitialPositioning.y + maxHeight), lookAt.localPosition.z);
        
        lookAt.localPosition += offsetY * Time.deltaTime;
    }

    void MovePlayer()
    {

        Vector3 forwardDirection = camTransform.forward;
        playerOrientation.forward = forwardDirection.normalized;

        if (animating)
        {
            moveVector = Vector3.zero;
        }

        if (!animating)
        {
            moveVector = playerInput.x * playerOrientation.right + playerInput.y * playerOrientation.forward;
        }


        moveVector.Normalize();

        if (isGrounded && moveVector != Vector3.zero)
        {
            if (isRunning)
            {
                if (!jump && !crouch && canChange)
                {
                    anim.SetBool("run", true);
                    anim.SetBool("walk", false);
                    anim.SetBool("idle", false);
                    running = true;
                }

                if (!crouch)
                {
                    moveSpeed = runSpeed;
                }
                if(crouch)
                {
                    moveSpeed = crouchSpeed;
                }

                            }
            if (!isRunning)
            {
                if (!jump && !crouch && canChange)
                {
                    anim.SetBool("walk", true);
                    anim.SetBool("run", false);
                    anim.SetBool("idle", false);
                    running = false;
                }
                if (!crouch)
                {
                    moveSpeed = walkSpeed;
                }
                if (crouch)
                {
                    moveSpeed = crouchSpeed;
                }

            }
        }


        if (moveVector == Vector3.zero && !jump && !crouch && canChange)
        {
            anim.SetBool("idle", true);
            anim.SetBool("walk", false);
            anim.SetBool("run", false);
            running = false;
        }

        if (moveVector != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveVector, Time.deltaTime * turnSpeed);
        }

        playerCharacter.transform.Translate(moveVector * moveSpeed * Time.deltaTime, Space.World);
    }

    public void SetIsRunning(bool _isrunning)
    {
        isRunningStick = _isrunning;
    }

    public void OnScreenTouch(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            canSwipe = true;
        }
        if (_context.canceled)
        {
            canSwipe = false;
        }
    }
    public void TouchScreenInitialPosition(InputAction.CallbackContext _context)
    {
        TouchScreenInitialPos = _context.ReadValue<Vector2>();
        TouchScreenCurrentPos = TouchScreenInitialPos;
    }
    public void TouchScreenCurrentPosition(InputAction.CallbackContext _context)
    {
        TouchScreenCurrentPos = _context.ReadValue<Vector2>();
    }
    public void MousePosRotation(InputAction.CallbackContext _context)
    {
        mousePos = _context.ReadValue<Vector2>();
    }
    public void RightStick(InputAction.CallbackContext _context)
    {
        rightStickVector = _context.ReadValue<Vector2>();
    }
    public void JumpButton(InputAction.CallbackContext _context)
    {
        if(_context.performed)
        {
            if (crouch)
            {
                jump = false;
            }
            if (!crouch)
            {
                jump = true;
            }
            Jump();
        }
    }
    public void CrouchButton(InputAction.CallbackContext _context)
    {
        if (!_context.performed)
        {
            crouch = !crouch;
            canChange = false;
            Crouching();
        }
    }
    void Crouching()
    {
        if (isGrounded && crouch)
        {
            anim.SetBool("idle", false);
            anim.SetBool("walk", false);
            anim.SetBool("run", false);
            if (!running && !canSlide)
            {
                anim.SetTrigger("crouch");
                animating = true;
            }
            if (running && canSlide)
            {
                anim.SetTrigger("slide");
                sliding = true;
                animating = true;
                rb.AddForce(slideForce * transform.forward, ForceMode.Impulse);
            }
        }
        if (isGrounded && !crouch)
        {
            anim.SetBool("idle", true);
        }
    }
    void Jump()
    {
        canChange = true;
        if(isGrounded && jump && !crouch)
        {
            
            anim.SetBool("idle", false); 
            anim.SetBool("walk", false); 
            anim.SetBool("run", false);

            if(canClimb)
            {

                posX = transform.position.x;
                posZ = transform.position.z;
                rb.isKinematic = true;
                capCollider.enabled = false;
                if (capCollider.height < posY && !jumpOverClimb)
                {
                    transform.position = transform.position + ((posY - capCollider.height) * transform.up);
                }
                if (capCollider.height >= posY && !jumpOverClimb)
                {
                    transform.position = transform.position + ((posY - horizontalHitPos) * transform.up);
                }
                if (normalClimb)
                {
                    anim.SetTrigger("normalClimb");
                    animating = true;
                }

                if (hangToClimb)
                {
                    anim.SetTrigger("jumpToHang");
                    animating = true;
                }

                if (jumpOverClimb)
                {
                    anim.SetTrigger("jumpOver");
                    animating = true;
                }
            }

            if (!canClimb)
            {
                animating = false;
                rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
                anim.SetTrigger("jump");
            }

            
        }
        if (!jump && crouch && !animating)
        {
            anim.SetBool("idle", true);
            crouch = false;
            animating = false;
        }
    }
    void RightStickRotation()
    {
        direction = rightStickVector;
        if(direction.x > 0)
        {
            dir = 1f;
        }
        
        if (direction.x < 0)
        {
            dir = -1f;
        }
        if (direction.x == 0)
        {
            dir = 0f;
        }
        if (direction.y > 0)
        {
            offsetY = new Vector3(0, 1, 0);
        }
        if (direction.y < 0)
        {
            offsetY = new Vector3(0, -1, 0);
        }
        if (direction.y == 0)
        {
            offsetY = new Vector3(0, 0, 0);
        }
    }
 /*   void TouchScreenRotation()
    {
        if (TouchScreenInitialPos.x >= Screen.width / 2 && canSwipe) {
            if (TouchScreenCurrentPos.x >= Screen.width / 2)
            {
                direction = TouchScreenCurrentPos - TouchScreenTemporaryPos;
            }
            else
            {
                direction = Vector2.zero;
            }
            if (direction.x > 0)
            {
                dir = 1f;
            }
            if (direction.x < 0)
            {
                dir = -1f;
            }
            if (direction.x == 0)
            {
                dir = 0f;
            }
            if (direction.y > 0)
            {
                offsetY = new Vector3(0, 1, 0);
            }
            if (direction.y < 0)
            {
                offsetY = new Vector3(0, -1, 0);
            }
            if (direction.y == 0)
            {
                offsetY = new Vector3(0, 0, 0);
            }


            TouchScreenTemporaryPos = TouchScreenCurrentPos;
        }
        if (!canSwipe)
        {
            dir = 0f;
            offsetY = new Vector3(0, 0, 0);
        }

    }
    void MouseRotation()
    {
        direction = mousePos - mouseTempPos;
        if (direction.x > 0)
        {
            dir = 1f;
        }
        if (direction.x < 0)
        {
            dir = -1f;
        }
        if (direction.x == 0)
        {
            dir = 0f;
        }
        if (direction.y > 0)
        {
            offsetY = new Vector3(0, 1, 0);
        }
        if (direction.y < 0)
        {
            offsetY = new Vector3(0, -1, 0);
        }
        if (direction.y == 0)
        {
            offsetY = new Vector3(0, 0, 0);
        }
        mouseTempPos = mousePos;
    }*/
    public void Jumped()
    {
        jumped = true;
    }

    public void EndOfNormalClimb()
    {
        jumped = true;
        transform.position = new Vector3(posX, posY, posZ) + transform.forward;
        anim.SetBool("idle", true);
        rb.isKinematic = false;
        capCollider.enabled = true;
        animating = false;
    }
    public void EndOfJumpToHang()
    {
        transform.position = transform.position + new Vector3(0, capCollider.height / 4, 0);
        anim.SetTrigger("hangIdle");
    }

    public void EndOfHangIdle()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("hangToClimb");
        climb = true;
    }

    public void EndOfHangToDrop()
    {
        animating = false;
        anim.applyRootMotion = false;
        anim.SetBool("idle", true);
        climb = false;
        jumped = true;
        rb.isKinematic = false;
        capCollider.enabled = true;
    }
    public void EndOfHangToClimb()
    {
        animating = false;
        anim.applyRootMotion = false;
        anim.SetBool("idle", true);
        climb = false;
        jumped = true;
        rb.isKinematic = false;
        capCollider.enabled = true;
    }
    public void EndOfJumpOverClimb()
    {
        animating = false;
        jumped = true;
        transform.position = new Vector3(posX, posY, posZ) + (0.8f * transform.forward);
        anim.SetBool("idle", true);
        rb.isKinematic = false;
        capCollider.enabled = true;
    }

    public void CanChange()
    {
        canChange = true;
    }
    public void AnimatingTrue()
    {
        animating = true;
    }
    public void AnimatingFalse()
    {
        animating = false;  
    }
    public void EndOfSlideAnimation()
    {
        capCollider.height = capsuleHeight;
        capCollider.center = new Vector3(capCollider.center.x, capsuleHeight / 2f, capCollider.center.z);
        animating = false;
        canChange = true;
        sliding = false;
        crouch = false;
        rb.velocity = Vector3.zero;
        anim.SetBool("idle", true);
    }
    public void EndOfIdleToCrouch()
    {
        capCollider.height = capsuleHeight / 1.3f;
        capCollider.center = new Vector3(capCollider.center.x, capsuleHeight / 2.6f, capCollider.center.z);
    }
    public void EndOfCrouchToIdle()
    {
        capCollider.height = capsuleHeight;
        capCollider.center = new Vector3(capCollider.center.x, capsuleHeight / 2f, capCollider.center.z);
    }

    public void StartSlideAnimation()
    {
        capCollider.height = capsuleHeight / 2.5f;
        capCollider.center = new Vector3(capCollider.center.x, capsuleHeight / 5f, capCollider.center.z);
    }

}