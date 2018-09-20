using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //In the future we will be able to click on planets and follow them.

    public GameObject objTarget;
    private Vector3 position;
    private Vector3 targetPosition;

    //private float lerpDistance = 1.0f;

    private void FixedUpdate()
    {
        position = transform.position;
        targetPosition = objTarget.transform.position;
        targetPosition.z = position.z;

        //Vector3 direction = targetPosition - transform.position;

        //if (direction.sqrMagnitude < lerpDistance * lerpDistance)
        //{
        //  Debug.Log("SLERP");
            position = Vector3.Slerp(position, targetPosition, objTarget.GetComponent<PhysicsProperties>().lastVelocity.magnitude * Time.fixedDeltaTime);
        //}
        //else
        //{
        //    Debug.Log("NO SLERP");
        //    position += (direction.normalized * objTarget.GetComponent<PhysicsProperties>().lastVelocity.magnitude);
        //}

        transform.position = position;
    }
}
