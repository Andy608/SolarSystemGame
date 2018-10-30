using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Gravitate))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PhysicsProperties))]
[RequireComponent(typeof(OrbitTracker))]
public class SpaceObject : MonoBehaviour
{
    private bool isJustSpawned = true;
    private bool isPaused = false;
    private bool isSelected = false;

    private Color objectColor;

    [HideInInspector] public Rigidbody2D objRigidbody;
    [HideInInspector] public Gravitate objGravitate;
    [HideInInspector] public PhysicsProperties objPhysicsProperties;
    [HideInInspector] public OrbitTracker objOrbitTracker;
    //[HideInInspector] public OrbitTrail objOrbitTrail;
    public SpaceObjectType objSpaceObjectType;

    public Color ObjectColor { get { return objectColor; } }

    private void OnEnable()
    {
        //objOrbitTrail = transform.GetChild(0).GetComponent<OrbitTrail>();
        objGravitate = GetComponent<Gravitate>();
        objRigidbody = GetComponent<Rigidbody2D>();
        objPhysicsProperties = GetComponent<PhysicsProperties>();
        objOrbitTracker = GetComponent<OrbitTracker>();
        objectColor = GetComponent<SpriteRenderer>().color;

        Managers.ObjectTracker.Instance.RegisterObject(this);
    }

    private void OnDisable()
    {
        if (Managers.ObjectTracker.Instance)
        {
            Managers.ObjectTracker.Instance.UnRegisterObject(this);
        }
    }

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }

        set
        {
            isPaused = value;
        }
    }

    public bool IsJustSpawned
    {
        get
        {
            return isJustSpawned;
        }
        
        set
        {
            //If the object is old, don't allow for it to be set to new again.
            if (!isJustSpawned)
            {
                return;
            }

            isJustSpawned = value;
        }
    }

    public bool IsSelected
    {
        get
        {
            return isSelected;
        }

        set
        {
            isSelected = value;
        }
    }
}
