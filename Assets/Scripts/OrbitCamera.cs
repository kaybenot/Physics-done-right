using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera), typeof(PlayerInput))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform focus;
    [SerializeField, Range(1f, 20f)] private float distance = 5f;
    [SerializeField, Min(0f)] private float focusRadius = 1f;
    [SerializeField, Range(0f, 1f)] private float focusCentering = 0.5f;
    [SerializeField, Range(1f, 360f)] private float rotationSpeed = 90f;
    [SerializeField, Range(-89f, 89f)] private float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    [SerializeField, Min(0f)] private float alignDelay = 5f;
    [SerializeField, Range(0f, 90f)] private float alignSmoothRange = 45f;
    [SerializeField] private LayerMask obstructionMask = -1;
    
    private Camera regularCamera;
    private Vector3 focusPoint, previousFocusPoint;
    private Vector2 orbitAngles = new Vector2(45f, 0f);
    private Vector2 input;
    private float lastManualRotationTime;

    private Vector3 cameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
            maxVerticalAngle = minVerticalAngle;
    }
    
    void Awake()
    {
        regularCamera = GetComponent<Camera>();
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
    }
    
    void Update()
    {
        updateFocusPoint();
        Quaternion lookRotation;
        if (manualRotation() || automaticRotation())
        {
            constrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
            lookRotation = transform.localRotation;
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;
        Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine.normalized;
        if (Physics.BoxCast(castFrom, cameraHalfExtends, castDirection, out RaycastHit hit, lookRotation,
            castDistance, obstructionMask))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }
            
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    static float getAngle(Vector2 direction)
    {
        float angle =  Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

    private bool automaticRotation()
    {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay)
            return false;
        Vector2 movement = new Vector2(
            focusPoint.x - previousFocusPoint.x,
            focusPoint.z - previousFocusPoint.z);
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.0001f)
            return false;
        float headingAngle = getAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs < alignSmoothRange)
            rotationChange *= deltaAbs / alignSmoothRange;
        else if (180f - deltaAbs < alignSmoothRange)
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    private bool manualRotation()
    {
        const float e = 0.001f;
        if (input.x < -e || input.x > e || input.y < -e || input.y > e)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }

    private void constrainAngles()
    {
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);
        if (orbitAngles.y < 0f)
            orbitAngles.y += 360f;
        else if (orbitAngles.y >= 360f)
            orbitAngles.y -= 360f;
    }

    private void updateFocusPoint()
    {
        previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0f)
        {
            float dist = Vector3.Distance(targetPoint, focusPoint);
            float t = 1f;
            if (dist > 0.01f && focusCentering > 0f)
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            if (dist > focusRadius)
                t = Mathf.Min(t, focusRadius / dist);
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else
            focusPoint = targetPoint;
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        if (context.performed)
            input = context.ReadValue<Vector2>();
        if (context.canceled)
            input = Vector2.zero;
    }
}
