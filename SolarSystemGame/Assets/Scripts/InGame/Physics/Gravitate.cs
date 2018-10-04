using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsProperties))]
public class Gravitate : MonoBehaviour
{
    public delegate void ObjectEnteredGravitationalPullAction(SpaceObject parent, SpaceObject child);
    public static event ObjectEnteredGravitationalPullAction OnObjectEnteredGravitationalPull;

    public delegate void ObjectExitedGravitationalPullAction(SpaceObject parent, SpaceObject child);
    public static event ObjectExitedGravitationalPullAction OnObjectExitedGravitationalPull;

    private static readonly float MIN_THRESHOLD = 0.01f;

    //Gravitational constant
    private static readonly float G = 0.0667408f;

    public SpaceObject objSpaceObj;
    public PhysicsProperties objPhysicsProperties;

    private List<Gravitate> gravitatingObjects = new List<Gravitate>();

    private void Start()
    {
        objSpaceObj = GetComponent<SpaceObject>();
        objPhysicsProperties = GetComponent<PhysicsProperties>();
    }

    private void OnEnable()
    {
        PhysicsProperties.OnAbsorbed += RemoveObjectFromPull;
    }

    private void OnDisable()
    {
        PhysicsProperties.OnAbsorbed -= RemoveObjectFromPull;
    }

    private void RemoveObjectFromPull(SpaceObject absorber, SpaceObject absorbed)
    {
        if (gravitatingObjects.Contains(absorbed.objGravitate))
        {
            gravitatingObjects.Remove(absorbed.objGravitate);
        }
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

            if (force.SqrMagnitude() > MIN_THRESHOLD)
            {
                if (!gravitatingObjects.Contains(objToGravitate))
                {
                    gravitatingObjects.Add(objToGravitate);

                    if (objSpaceObj.objRigidbody.mass >= objToGravitate.objSpaceObj.objRigidbody.mass)
                    {
                        if (OnObjectEnteredGravitationalPull != null)
                        {
                            OnObjectEnteredGravitationalPull(objSpaceObj, objToGravitate.objSpaceObj);
                        }
                    }
                }
            }
            else if (gravitatingObjects.Contains(objToGravitate))
            {
                gravitatingObjects.Remove(objToGravitate);

                if (OnObjectExitedGravitationalPull != null)
                {
                    if (objSpaceObj.objRigidbody.mass >= objToGravitate.objSpaceObj.objRigidbody.mass)
                    {
                        Debug.Log("OUT OF GRAVITATIONAL RANGE");
                        OnObjectExitedGravitationalPull(objSpaceObj, objToGravitate.objSpaceObj);
                    }
                }
            }
        }
        else
        {
            //Absorb!
            PhysicsProperties.AbsorbObject(objSpaceObj, objToGravitate.objSpaceObj);
        }
    }
}
