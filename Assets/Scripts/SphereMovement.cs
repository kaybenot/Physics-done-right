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
    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 25f, maxStairsAngle = 50f;
    [SerializeField, Range(0f, 100f)] private float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)] private float probeDistance = 1f;
    [SerializeField] private LayerMask probeMask = -1, stairsMask = -1;

    private Vector3 velocity;
    private Vector3 acceleration;
    private Rigidbody body;
    private int jumpPhase = 0;
    private float minGroundDotProduct, minStairsDotProduct;
    private Vector3 contactNormal;
    private int groundContactCount = 0;
    private int stepsSinceLastGrounded = 0, stepsSinceLastJump = 0;

    public bool OnGround => groundContactCount > 0;

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
    }

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        OnValidate();
    }

    void FixedUpdate()
    {
        updateState();
        adjustVelocity();
        body.velocity = velocity;
        clearState();
    }

    void Update()
    {
        GetComponent<Renderer>().material.SetColor("_BaseColor", OnGround ? Color.black : Color.white);
    }

    void OnCollisionEnter(Collision other)
    {
        evaluateCollision(other);
    }

    void OnCollisionStay(Collision other)
    {
        evaluateCollision(other);
    }

    private float getMinDot(int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
    }

    private void clearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    private void updateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (OnGround || snapToGround())
        {
            stepsSinceLastGrounded = 0;
            jumpPhase = 0;
        }
    }

    private bool snapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 3)
            return false;
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
            return false;
        if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask))
            return false;
        if (hit.normal.y < getMinDot(hit.collider.gameObject.layer))
            return false;
        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if(dot > 0f)
            velocity = (velocity - hit.normal * dot).normalized * speed;
        return true;
    }

    private void evaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            float minDot = getMinDot(collision.gameObject.layer);
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minDot)
            {
                groundContactCount += 1;
                contactNormal += normal;
                if(groundContactCount > 1)
                    contactNormal.Normalize();
            }
        }
    }

    private void adjustVelocity()
    {
        Vector3 xAxis = projectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = projectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float realAcceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = realAcceleration * Time.fixedDeltaTime;
        float newX = Mathf.MoveTowards(currentX, acceleration.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, acceleration.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private Vector3 projectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && (OnGround || jumpPhase < maxAirJumps))
        {
            stepsSinceLastJump = 0;
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if(alignedSpeed > 0f)
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            body.velocity += contactNormal * jumpSpeed;
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
