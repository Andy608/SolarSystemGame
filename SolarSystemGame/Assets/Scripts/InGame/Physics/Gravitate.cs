using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpaceObject))]
[RequireComponent(typeof(PhysicsProperties))]
public class Gravitate : MonoBehaviour
{
    public delegate void ObjectEnteredGravitationalPullAction(SpaceObject firstObj, SpaceObject secondObj);
    public static event ObjectEnteredGravitationalPullAction OnObjectEnteredGravitationalPull;

    public delegate void ObjectExitedGravitationalPullAction(SpaceObject firstObj, SpaceObject secondObj);
    public static event ObjectExitedGravitationalPullAction OnObjectExitedGravitationalPull;

    private static readonly float MIN_THRESHOLD = 0.01f;

    //Gravitational constant
    public static readonly float G = 0.0667408f;

    private SpaceObject objSpaceObj;
    private PhysicsProperties objPhysicsProperties;

    public SpaceObject ObjSpaceObj { get { return objSpaceObj; } }
    public PhysicsProperties ObjPhysicsProperties { get { return objPhysicsProperties; } }

    private List<Gravitate> gravitatingObjects = new List<Gravitate>();

    private void Start()
    {
        objSpaceObj = GetComponent<SpaceObject>();
        objPhysicsProperties = GetComponent<PhysicsProperties>();
    }

    private void OnEnable()
    {
        Managers.UniversePlaySpaceManager.OnObjectDestroyed += RemoveObjectFromPull;
    }

    private void OnDisable()
    {
        Managers.UniversePlaySpaceManager.OnObjectDestroyed -= RemoveObjectFromPull;
    }

    private void RemoveObjectFromPull(SpaceObject destroyedObj)
    {
        if (gravitatingObjects.Contains(destroyedObj.objGravitate))
        {
            gravitatingObjects.Remove(destroyedObj.objGravitate);
        }
    }

    public void ApplyGravity(Gravitate objToGravitate)
    {
        Rigidbody2D rbToGravitate = objToGravitate.objSpaceObj.objRigidbody;

        Vector2 direction = objSpaceObj.objRigidbody.position - rbToGravitate.position;
        float distanceSquared = direction.sqrMagnitude;

        float distanceFromCenter = (objPhysicsProperties.Radius + objToGravitate.objPhysicsProperties.Radius);

        //Debug.Log("Dist from center: " + distanceFromCenter + " | Distance: " + direction.magnitude);

        if (direction.sqrMagnitude > distanceFromCenter * distanceFromCenter)
        {
            float gravitationMag = G * (objSpaceObj.objRigidbody.mass * rbToGravitate.mass) / distanceSquared;
            Vector2 force = direction.normalized * gravitationMag;

            rbToGravitate.AddForce(force, ForceMode2D.Force);

            if (force.sqrMagnitude > MIN_THRESHOLD)
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
            Managers.UniversePlaySpaceManager.AbsorbObject(objSpaceObj, objToGravitate.objSpaceObj);
        }
    }
}
