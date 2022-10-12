using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereMovement : MonoBehaviour
{
    [Range(0f, 100f)] public float MaxSpeed = 10f;
    [Range(0f, 100f)] public float MaxAcceleration = 10f;
    [Range(0f, 1f)] public float Bounciness = 0.5f;
    public Rect AllowedArea = new Rect(-5f, -5f, 10f, 10f);
    
    private Vector3 velocity;
    private Vector3 acceleration;
    
    private void FixedUpdate()
    {
        float maxSpeedChange = MaxAcceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, acceleration.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, acceleration.z, maxSpeedChange);
        Vector3 newPos = transform.localPosition + velocity * Time.fixedDeltaTime;
        if (!AllowedArea.Contains(new Vector2(newPos.x, newPos.z)))
        {
            if (newPos.x < AllowedArea.xMin)
            {
                newPos.x = AllowedArea.xMin;
                velocity.x = -velocity.x * Bounciness;
            }
            else if (newPos.x > AllowedArea.xMax)
            {
                newPos.x = AllowedArea.xMax;
                velocity.x = -velocity.x * Bounciness;
            }

            if (newPos.z < AllowedArea.yMin)
            {
                newPos.z = AllowedArea.yMin;
                velocity.z = -velocity.z * Bounciness;
            }
            else if (newPos.z > AllowedArea.yMax)
            {
                newPos.z = AllowedArea.yMax;
                velocity.z = -velocity.z * Bounciness;
            }
        }
        transform.localPosition = newPos;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var read = context.ReadValue<Vector2>();
            acceleration = new Vector3(read.x, 0f, read.y);
            acceleration = Vector3.ClampMagnitude(acceleration, 1f) * MaxSpeed;
        }
        
        if(context.canceled)
            acceleration = Vector3.zero;
    }
}
