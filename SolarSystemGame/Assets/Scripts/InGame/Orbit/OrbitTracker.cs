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
        Managers.UniversePlaySpaceManager.OnAbsorbed += UnTrackFromAbsorption;

        Managers.UniversePlaySpaceManager.OnObjectDestroyed += UnTrackFromDeletion;
    }

    public void OnDisable()
    {
        Gravitate.OnObjectEnteredGravitationalPull -= ShouldTrackOrbit;

        Gravitate.OnObjectExitedGravitationalPull -= UnTrackFromGravitationalPull;
        Managers.UniversePlaySpaceManager.OnAbsorbed -= UnTrackFromAbsorption;

        Managers.UniversePlaySpaceManager.OnObjectDestroyed -= UnTrackFromDeletion;
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
        UnTrackFromAllOrbits(child);
        ////All objects untrack the child that was absorbed.
        //UnTrackOrbit(parent, child, EnumOrbitalType.CHILD);
        //UnTrackOrbit(parent, child, EnumOrbitalType.PARENT);

        ////If this object is the child, untrack all the parents.
        //if (child == objSpaceObject)
        //{
        //    parentList.Clear();
        //}

        ////If the parent's new mass is now bigger than the current object, untrack it as a child and track it as a parent.
        //if (parent.objRigidbody.mass > objSpaceObject.objRigidbody.mass)
        //{
        //    //UnTrackOrbit(parent, child, EnumOrbitalType.CHILD);
        //    //TrackOrbit(parent, child, EnumOrbitalType.PARENT);
        //}
    }

    private void UnTrackFromDeletion(SpaceObject deletedObj)
    {
        //All objects should untrack this object.
        UnTrackFromAllOrbits(deletedObj);
    }

    private void UnTrackFromGravitationalPull(SpaceObject parent, SpaceObject child)
    {

        ////If this object is the parent, untrack the child that is out of range.
        //if (parent == objSpaceObject)
        //{
        //    Debug.Log("Untrack: " + parent.objSpaceObjectType.Type.ToString() + " | " + child.objSpaceObjectType.Type.ToString());
        //    UnTrackOrbit(parent, child, EnumOrbitalType.CHILD);
        //}
        ////If this object is the child, untrack the parent that is out of range.
        //else if (child == objSpaceObject)
        //{
        //    UnTrackOrbit(parent, child, EnumOrbitalType.PARENT);
        //}

        //If this object is the child, untrack the parent that is out of range.
        //if (child == objSpaceObject)
        //{
        //    UnTrackOrbit(parent, child, EnumOrbitalType.CHILD);
        //}

        //Untrack this child object from all parents.
        UnTrackFromAllOrbits(child);
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
                orbitList.RemoveAt(i);
                break;
            }
        }

        //If this object has no more parents anymore, delete it
        //This might fail if the object has other objects as children.
        if (parentList.Count == 0)
        {
            Destroy(child.gameObject);
        }
    }

    private void UnTrackFromAllOrbits(SpaceObject obj)
    {
        int i = 0;
        for (; i < childList.Count; ++i)
        {
            if (childList[i].OrbitChild.GetInstanceID() == obj.GetInstanceID())
            {
                childList.RemoveAt(i);
                break;
            }
        }

        for (; i < parentList.Count; ++i)
        {
            if (parentList[i].OrbitParent.GetInstanceID() == obj.GetInstanceID())
            {
                parentList.RemoveAt(i);
                break;
            }
        }
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
