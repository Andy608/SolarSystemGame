using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsProperties))]
public class Gravitate : MonoBehaviour
{
    //Gravitational constant
    private static readonly float G = 0.0667408f;

    //The list of all the objects that are affected by gravity.
    private static List<Gravitate> gravitators;

    private PhysicsProperties objPhysicsProperties;

    private void Start()
    {
        objPhysicsProperties = GetComponent<PhysicsProperties>();
    }

    private void OnEnable()
    {
        if (gravitators == null)
        {
            gravitators = new List<Gravitate>();
        }

        gravitators.Add(this);
    }

    private void OnDisable()
    {
        gravitators.Remove(this);
    }

	private void FixedUpdate ()
    {
        for (int i = 0; i < gravitators.Count; ++i)
        {
            Gravitate gravitate = gravitators[i];

            if (gravitate != this)
            {
                ApplyGravity(gravitate);
            }
        }

        objPhysicsProperties.UpdatePhysicsProperties();
    }

    private void ApplyGravity(Gravitate objToGravitate)
    {
        Rigidbody2D rbToGravitate = objToGravitate.objPhysicsProperties.objRigidbody;

        Vector2 direction = objPhysicsProperties.objRigidbody.position - rbToGravitate.position;
        float distanceSquared = direction.sqrMagnitude;

        //Radius is half the scale. This doesn't work because world units are different from scale.
        float distanceFromCenter = (objPhysicsProperties.Radius + objToGravitate.objPhysicsProperties.Radius);

        //Debug.Log("Dist from center: " + distanceFromCenter + " | Distance: " + direction.magnitude);

        if (direction.magnitude > distanceFromCenter)
        {
            float gravitationMag = G * (objPhysicsProperties.objRigidbody.mass * rbToGravitate.mass) / distanceSquared;
            Vector2 force = direction.normalized * gravitationMag;

            rbToGravitate.AddForce(force, ForceMode2D.Force);
        }
        else
        {
            //Absorb!
            PhysicsProperties.AbsorbObject(objPhysicsProperties, objToGravitate.objPhysicsProperties);
        }
    }
}
