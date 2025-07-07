using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementController : MonoBehaviour
{
    private float crouchSpeed = 4;
    private float walkSpeed = 6;
    private float runSpeed = 8;

    [Header("Jump Settings")]
    private float jumpForce = 5f;
    private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;
    public Transform groundCheck; 
    private float gravity = -20f; 
    private Vector3 velocity; 

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ConfigurePhysics();
    }

    private void ConfigurePhysics()
    {
        rb.useGravity = false; 
        rb.drag = 5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        HandleInput();
        CheckGroundStatus();
    }

    private void FixedUpdate()
    {
        HandleGravity(); 
        HandleMovement();
        HandleJump();
    }

    private void HandleGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f; 
        }
        if (!isGrounded || velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, velocity.y, rb.velocity.z);
        }
    }

    private void HandleInput()
    {
        moveDirection = CalculateMovementDirection();
    }

    private Vector3 CalculateMovementDirection()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        return (forward * VerticalAxis() + right * HorizontalAxis()).normalized;
    }

    private void HandleMovement()
    {
        float currentSpeed = GetTargetSpeed();
        Vector3 targetVelocity = moveDirection * currentSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    private void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (JumpInputPressed() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
        }
    }


    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
    }

    #region Métodos de input 
    private float HorizontalAxis()
    {
        return Input.GetAxis("Horizontal");
    }

    private float VerticalAxis()
    {
        return Input.GetAxis("Vertical");
    }

    private bool JumpInputPressed()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool RunInputPressed()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    private bool CrouchInputPressed()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }
    #endregion  

    private float GetTargetSpeed()
    {
        if (RunInputPressed()) return runSpeed;
        if (CrouchInputPressed()) return crouchSpeed;
        return walkSpeed;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
    }
}