using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform focus;
    [SerializeField, Range(1f, 20f)] private float distance = 5f;
    
    void Start()
    {
        
    }
    
    void LateUpdate()
    {
        Vector3 focusPoint = focus.position;
        Vector3 lookDirection = transform.forward;
        transform.localPosition = focusPoint - lookDirection * distance;
    }
}
