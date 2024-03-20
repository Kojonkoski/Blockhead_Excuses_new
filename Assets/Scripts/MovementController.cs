using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerCharacter = GameObject.FindWithTag("Player");
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

        if (playerInput != Vector2.zero) {
            moveSpeed = walkSpeed;
        }
        if (isRunningKey || isRunningStick) {
            isRunning = true;
        }
        else if (!isRunningKey && !isRunningStick) {
            isRunning = false;
        }
    }

    void MovePlayer() 
    {
        Vector3 moveVector = playerInput.x * Vector3.right + playerInput.y * Vector3.forward;
        moveVector.Normalize();

        if (isGrounded && moveVector !=Vector3.zero) 
        {
            if (isRunning)
            {
                moveSpeed = runSpeed;
                anim.SetTrigger("run");
            }
            if (!isRunning)
            {
                moveSpeed = walkSpeed;
                anim.SetTrigger("walk");
            }
        }
        

        if(moveVector == Vector3.zero)
        {
            anim.SetTrigger("idle");
        }

        playerCharacter.transform.Translate(moveVector * moveSpeed * Time.deltaTime, Space.World);
    }




}
