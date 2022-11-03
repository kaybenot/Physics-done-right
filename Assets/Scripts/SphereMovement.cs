using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f, maxAirAcceleration = 1f;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 2f;
    [SerializeField, Range(0, 5)] private int maxAirJumps = 0;
    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 25f;

    private Vector3 velocity;
    private Vector3 acceleration;
    private Rigidbody body;
    private bool onGround;
    private int jumpPhase = 0;
    private float minGroundDotProduct;

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        OnValidate();
    }

    void FixedUpdate()
    {
        updateState();
        float realAcceleration = onGround ? maxAcceleration : maxAirAcceleration;
        onGround = false;
        float maxSpeedChange = realAcceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, acceleration.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, acceleration.z, maxSpeedChange);
        body.velocity = velocity;
    }

    private void updateState()
    {
        velocity = body.velocity;
        if (onGround)
            jumpPhase = 0;
    }

    void OnCollisionEnter(Collision other)
    {
        evaluateCollision(other);
    }

    void OnCollisionStay(Collision other)
    {
        evaluateCollision(other);
    }

    private void evaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= minGroundDotProduct;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && (onGround || jumpPhase < maxAirJumps))
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            if(velocity.y > 0f)
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            body.velocity += new Vector3(0f, jumpSpeed, 0f);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var read = context.ReadValue<Vector2>();
            acceleration = new Vector3(read.x, 0f, read.y);
            acceleration = Vector3.ClampMagnitude(acceleration, 1f) * maxSpeed;
        }
        
        if(context.canceled)
            acceleration = Vector3.zero;
    }
}
