using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumOrbitalType
{
    CHILD,
    PARENT
}

//Shouldn't be called OrbitTracker. Should be called SpaceObjectOrbit
[RequireComponent(typeof(SpaceObject))]
public class OrbitTracker : MonoBehaviour
{
    private SpaceObject objSpaceObject;

    private List<OrbitData> childList = new List<OrbitData>();
    private List<OrbitData> parentList = new List<OrbitData>();

    public List<OrbitData> ChildList { get { return childList; } }
    public List<OrbitData> ParentList { get { return parentList; } }

    public void OnEnable()
    {
        objSpaceObject = GetComponent<SpaceObject>();

        Gravitate.OnObjectEnteredGravitationalPull += ShouldTrackOrbit;

        Gravitate.OnObjectExitedGravitationalPull += UnTrackFromGravitationalPull;
        PhysicsProperties.OnAbsorbed += UnTrackFromAbsorption;
    }

    public void OnDisable()
    {
        Gravitate.OnObjectEnteredGravitationalPull -= ShouldTrackOrbit;

        Gravitate.OnObjectExitedGravitationalPull -= UnTrackFromGravitationalPull;
        PhysicsProperties.OnAbsorbed -= UnTrackFromAbsorption;
    }

    private void ShouldTrackOrbit(SpaceObject parent, SpaceObject child)
    {
        //If this current obj is the parent obj, track the child orbit.
        if (parent == objSpaceObject)
        {
            TrackOrbit(parent, child, EnumOrbitalType.CHILD);
        }
        //If this current obj is the child obj, track the parent obj.
        else if (child == objSpaceObject)
        {
            TrackOrbit(child, parent, EnumOrbitalType.PARENT);
        }
    }

    private void UnTrackFromAbsorption(SpaceObject parent, SpaceObject child)
    {
        //All objects untrack the child that was absorbed.
        UnTrackOrbit(parent, child, EnumOrbitalType.CHILD);
        UnTrackOrbit(parent, child, EnumOrbitalType.PARENT);

        //If this object is the child, untrack all the parents.
        if (child == objSpaceObject)
        {
            parentList.Clear();
        }

        //If the parent's new mass is now bigger than the current object, untrack it as a child and track it as a parent.
        if (parent.objRigidbody.mass > objSpaceObject.objRigidbody.mass)
        {
            //UnTrackOrbit(parent, child, EnumOrbitalType.CHILD);
            //TrackOrbit(parent, child, EnumOrbitalType.PARENT);
        }
    }

    private void UnTrackFromGravitationalPull(SpaceObject parent, SpaceObject child)
    {
        //If this object is the parent, untrack the child that is out of range.
        if (parent == objSpaceObject)
        {
            UnTrackOrbit(parent, child, EnumOrbitalType.CHILD);
        }
        //If this object is the child, untrack the parent that is out of range.
        else if (child == objSpaceObject)
        {
            UnTrackOrbit(parent, child, EnumOrbitalType.PARENT);
        }
    }

    private void TrackOrbit(SpaceObject parent, SpaceObject child, EnumOrbitalType orbitalType)
    {
        List<OrbitData> orbitList;
        SpaceObject orbitObj;

        if (orbitalType == EnumOrbitalType.CHILD)
        {
            orbitList = childList;
            orbitObj = child;
        }
        else
        {
            orbitList = parentList;
            orbitObj = parent;
        }

        int i = 0;

        for (; i < orbitList.Count; ++i)
        {
            if (orbitList[i].GetOrbitObj(orbitalType).GetInstanceID() == orbitObj.GetInstanceID())
            {
                //Already tracking orbit.
                return;
            }
        }

        OrbitData data = new OrbitData(parent, child);
        orbitList.Add(data);
    }

    private void UnTrackOrbit(SpaceObject parent, SpaceObject child, EnumOrbitalType orbitalType)
    {
        List<OrbitData> orbitList;
        SpaceObject orbitObj;
        int i = 0;

        if (orbitalType == EnumOrbitalType.CHILD)
        {
            orbitList = childList;
            orbitObj = child;
        }
        else
        {
            orbitList = parentList;
            orbitObj = parent;
        }

        for (; i < orbitList.Count; ++i)
        {
            if (orbitList[i].GetOrbitObj(orbitalType).GetInstanceID() == orbitObj.GetInstanceID())
            {
                if (orbitalType == EnumOrbitalType.PARENT)
                    Debug.Log("REMOVING");

                orbitList.RemoveAt(i);
                break;
            }
        }

        //if (orbitalType == EnumOrbitalType.PARENT && orbitList.Count == 0 && Managers.ObjectTracker.Instance.ObjectsInUniverse.Count > 1)
        //{
        //    Destroy(child.gameObject);
        //    Debug.Log("The object is out of gravitational range! Destroying it.");
        //}

        //if (orbitalType == EnumOrbitalType.PARENT)
        //    Debug.Log("Parent List: " + orbitList.Count);

        //if (orbitalType == EnumOrbitalType.CHILD)
        //    Debug.Log("Child List: " + orbitList.Count);
    }

    private void FixedUpdate()
    {
        if (Managers.GameState.Instance.IsState(Managers.GameState.EnumGameState.RUNNING))
        {
            foreach (OrbitData data in childList)
            {
                data.UpdateOrbit();
            }
        }
    }
}
