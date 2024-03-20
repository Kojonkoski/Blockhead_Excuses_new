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

    private void Start()
    {
        playerCharacter = GameObject.FindWithTag("Player");
    }

    public void Move(InputAction.CallbackContext _context)
    {
        playerInput = _context.ReadValue<Vector2>();
    }


    public void Update() 
    {
        MovePlayer();

        if (playerInput != Vector2.zero) {
            moveSpeed = walkSpeed;
        }
    }

    void MovePlayer() 
    {
        Vector3 moveVector = playerInput.x * Vector3.right + playerInput.y * Vector3.forward;
        moveVector.Normalize();
        playerCharacter.transform.Translate(moveVector * moveSpeed * Time.deltaTime, Space.World);
    }




}
