using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    //Detection Point
    public Transform detectionPoint;
    //Detection Radius
    private const float detectionRadius = 0.2f;
    //Detection Layer
    public LayerMask detectionLayer;

    void Update()
    {
        if(DetectObject())
        {
            if(InteractInput())
            {
                Debug.Log("INTERACT");
            }
        }
    }

    bool InteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    bool DetectObject()
    {
        return Physics2D.OverlapCircle(detectionPoint.position,detectionRadius,detectionLayer);        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);
    }
}
