using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpaceObject))]
public class PhysicsProperties : MonoBehaviour
{
    public delegate void AbsorbedAction(SpaceObject absorber);
    public static event AbsorbedAction OnAbsorbed;

    //Different depending on the object type.
    public float radiusScaleCoefficient = 1.0f;

    public Vector2 initialVelocity = new Vector2();

    //NOT USED YET
    public float maxRadius = 5.0f;
    public float minRadius = 0.1f;

    //By default image is 100px by 100px
    public float imageWidth = 100;
    public float pixelsPerWorldUnit = 100.0f;

    private float currentScale = 1.0f;
    private Vector3 currentScaleAsVec = new Vector3();

    [HideInInspector] public Vector2 lastVelocity = new Vector2();

    [HideInInspector] public Vector2 lastAcceleration = new Vector2();
    [HideInInspector] public Vector2 acceleration = new Vector2();

    //Here for now - This will depend on each object type - get info from their script regarding density, etc.

    //scale = r * 2
    //r = 1 scale -> 10000 mass
    //r = 0.1 -> 100 mass
    //r = 0.01 => = 1 mass
    //mass = 100 * scale * scale
    //mass / 100 = scale * scale
    //scale = sqrt(mass / 100)

    public float initialMass;
    private float currentMass;

    [HideInInspector] public float Radius
    {
        get
        {
            return circumference / 2.0f;
        }
    }

    [HideInInspector] public float SqrRadius
    {
        get
        {
            float r = Radius;
            return r * r;
        }
    }

    [HideInInspector] public float circumference;


    [HideInInspector] public SpaceObject objSpaceObject;

    private void Start()
    {
        objSpaceObject = GetComponent<SpaceObject>();

        currentMass = initialMass;
        objSpaceObject.objRigidbody.mass = currentMass;

        objSpaceObject.objRigidbody.velocity = initialVelocity;

        UpdateRadiusAndScale();
    }

    private void OnValidate()
    {
        currentMass = initialMass;
        UpdateRadiusAndScale();
    }

    public void UpdatePhysicsProperties()
    {
        lastAcceleration = acceleration;
        acceleration = (objSpaceObject.objRigidbody.velocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = objSpaceObject.objRigidbody.velocity;
    }

    public static void AbsorbObject(SpaceObject obj1, SpaceObject obj2)
    {
        SpaceObject absorber;
        SpaceObject absorbed;

        if (obj1.objRigidbody.mass >= obj2.objRigidbody.mass)
        {
            absorber = obj1;
            absorbed = obj2;
        }
        else
        {
            absorber = obj2;
            absorbed = obj1;
        }

        //Add the impact force.
        absorber.objRigidbody.AddForce(absorbed.objRigidbody.velocity * absorbed.objPhysicsProperties.currentMass, ForceMode2D.Impulse);

        absorber.objPhysicsProperties.currentMass += absorbed.objPhysicsProperties.currentMass;
        absorber.objRigidbody.mass = absorber.objPhysicsProperties.currentMass;
        absorber.objPhysicsProperties.UpdateRadiusAndScale();

        //If the target is absorbed, make the new target the absorber.
        //if (absorbed == Managers.CameraState.Instance.objCameraMove.ObjTarget)
        //{
        if (OnAbsorbed != null)
        {
            OnAbsorbed(absorber);
        }

            //Debug.Log("NEW TARGET");
        //}

        Destroy(absorbed.gameObject);
    }

    private void UpdateRadiusAndScale()
    {
        UpdateRadius();
        UpdateScale();
    }

    private void UpdateRadius()
    {
        //The radius is in world units.
        circumference = Mathf.Pow(currentMass / (Mathf.PI * radiusScaleCoefficient), 0.33f);
    }

    public void UpdateScale()
    {
        //Radius is 1.
        //Image is 64 px.
        //PixelsPerWorldUnit = 100px.
        //Scale of 1 = imageWidth / PixelsPerWorldUnit
        //New scale = circumference / Scale of 1

        currentScale = circumference / (imageWidth / pixelsPerWorldUnit);

        currentScaleAsVec.x = currentScale;
        currentScaleAsVec.y = currentScale;

        gameObject.transform.localScale = currentScaleAsVec;
    }
}
