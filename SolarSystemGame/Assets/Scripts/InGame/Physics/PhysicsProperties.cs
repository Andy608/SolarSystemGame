using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpaceObject))]
[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsProperties : MonoBehaviour
{
    private float currentScale = 1.0f;
    private Vector3 currentScaleAsVec = new Vector3();

    private Vector2 initialVelocity = new Vector2();
    //private Vector2 lastVelocity = new Vector2();
    //private Vector2 lastAcceleration = new Vector2();
    //private Vector2 acceleration = new Vector2();

    private float currentMass;

    private float circumference;
    private SpaceObject objSpaceObject;

    public float Radius { get { return circumference / 2.0f; } }
    public float SqrRadius { get { float r = Radius; return r * r; } }
    public float CurrentMass { get { return currentMass; } set { currentMass = value; } }

    private void OnEnable()
    {
        objSpaceObject = GetComponent<SpaceObject>();

        currentMass = objSpaceObject.objSpaceObjectType.DefaultMass;
        objSpaceObject.objRigidbody.mass = currentMass;

        objSpaceObject.objRigidbody.velocity = initialVelocity;

        UpdateRadiusAndScale();
    }

    private void OnValidate()
    {
        GetComponent<Rigidbody2D>().mass = currentMass;
        objSpaceObject = GetComponent<SpaceObject>();
        currentMass = objSpaceObject.objSpaceObjectType.DefaultMass;

        //Debug.Log("HELLO: " + objSpaceObject.objSpaceObjectType);

        UpdateRadiusAndScale();
    }

    //public void UpdatePhysicsProperties()
    //{
    //    lastAcceleration = acceleration;
    //    acceleration = (objSpaceObject.objRigidbody.velocity - lastVelocity) / Time.fixedDeltaTime;
    //    lastVelocity = objSpaceObject.objRigidbody.velocity;
    //}

    public void UpdateRadiusAndScale()
    {
        UpdateRadius();
        UpdateScale();
    }

    private void UpdateRadius()
    {
        //The radius is in world units.
        circumference = Mathf.Pow(currentMass / (Mathf.PI * objSpaceObject.objSpaceObjectType.RadiusScaleMult), 0.33f);
    }

    public void UpdateScale()
    {
        //Radius is 1.
        //Image is 64 px.
        //PixelsPerWorldUnit = 100px.
        //Scale of 1 = imageWidth / PixelsPerWorldUnit
        //New scale = circumference / Scale of 1

        currentScale = circumference / (objSpaceObject.objSpaceObjectType.SpriteWidth / objSpaceObject.objSpaceObjectType.PixelsPerUnit);

        currentScaleAsVec.x = currentScale;
        currentScaleAsVec.y = currentScale;

        gameObject.transform.localScale = currentScaleAsVec;
    }
    
    public void UpdateMass(float mass)
    {
        currentMass = mass;
        objSpaceObject.objRigidbody.mass = currentMass;
        UpdateRadiusAndScale();


    }
}
