using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Gravitate))]
public class SpaceObject : MonoBehaviour
{
    //If an object is not just spawned, then drag input will not affect velocity 
    //and if it is clicked on, the option to delete it comes up.

    //If an object that is just spawned, then drag input will affect velocity.

    private bool isPaused = false;
    private bool justSpawned = true;
    private bool isSelected = false;

    [HideInInspector] public Rigidbody2D objRigidbody;
    [HideInInspector] public Gravitate objGravitate;
    [HideInInspector] public PhysicsProperties objPhysicsProperties;

    private void OnEnable()
    {
        objGravitate = GetComponent<Gravitate>();
        objRigidbody = GetComponent<Rigidbody2D>();
        objPhysicsProperties = GetComponent<PhysicsProperties>();

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

    public bool JustSpawned
    {
        get
        {
            return justSpawned;
        }
        
        set
        {
            //If the object is old, don't allow for it to be set to new again.
            if (!justSpawned)
            {
                return;
            }

            justSpawned = value;
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
