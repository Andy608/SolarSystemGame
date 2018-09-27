using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsProperties))]
public class Gravitate : MonoBehaviour
{
    //Gravitational constant
    private static readonly float G = 0.0667408f;

    public SpaceObject objSpaceObj;
    public PhysicsProperties objPhysicsProperties;

    private void Start()
    {
        objSpaceObj = GetComponent<SpaceObject>();
        objPhysicsProperties = GetComponent<PhysicsProperties>();
    }

    public void ApplyGravity(Gravitate objToGravitate)
    {
        Rigidbody2D rbToGravitate = objToGravitate.objSpaceObj.objRigidbody;

        Vector2 direction = objSpaceObj.objRigidbody.position - rbToGravitate.position;
        float distanceSquared = direction.sqrMagnitude;

        float distanceFromCenter = (objPhysicsProperties.Radius + objToGravitate.objPhysicsProperties.Radius);

        //Debug.Log("Dist from center: " + distanceFromCenter + " | Distance: " + direction.magnitude);

        if (direction.magnitude > distanceFromCenter)
        {
            float gravitationMag = G * (objSpaceObj.objRigidbody.mass * rbToGravitate.mass) / distanceSquared;
            Vector2 force = direction.normalized * gravitationMag;

            rbToGravitate.AddRelativeForce(force, ForceMode2D.Force);
        }
        else
        {
            //Absorb!
            PhysicsProperties.AbsorbObject(objSpaceObj, objToGravitate.objSpaceObj);
        }
    }
}
