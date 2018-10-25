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

    private void ShouldTrackOrbit(SpaceObject firstObj, SpaceObject secondObj)
    {
        if (firstObj == objSpaceObject)
        {
            if (firstObj.objRigidbody.mass > secondObj.objRigidbody.mass)
            {
                TrackChildOrbit(secondObj);
            }
            else if (firstObj.objRigidbody.mass == secondObj.objRigidbody.mass &&
                !secondObj.GetComponent<OrbitTracker>().IsTrackingChildOrbit(firstObj))
            {
                TrackChildOrbit(secondObj);
            }
            else
            {
                TrackParentOrbit(secondObj);
            }
        }
    }

    private void UnTrackFromAbsorption(SpaceObject absorber, SpaceObject absorbed)
    {
        UnTrackOrbitRelationship(absorbed);
        UnTrackOrbitRelationship(absorber);

        if (absorber != objSpaceObject)
        {
            if (absorber.objRigidbody.mass > objSpaceObject.objRigidbody.mass)
            {
                TrackParentOrbit(absorber);
            }
            else
            {
                TrackChildOrbit(absorber);
            }
        }
    }

    private void UnTrackFromDeletion(SpaceObject deletedObj)
    {
        UnTrackOrbitRelationship(deletedObj);
    }

    private void UnTrackFromGravitationalPull(SpaceObject parent, SpaceObject child)
    {
        if (parent == objSpaceObject)
        {
            UnTrackOrbitRelationship(child);
        }
        else if (child == objSpaceObject)
        {
            UnTrackOrbitRelationship(parent);
        }
    }

    private void TrackParentOrbit(SpaceObject parent)
    {
        int i = 0;
        for (; i < parentList.Count; ++i)
        {
            if (parentList[i].OrbitParent.GetInstanceID() == parent.GetInstanceID())
            {
                //Already tracking orbit.
                return;
            }
        }

        OrbitData data = new OrbitData(parent, objSpaceObject);
        parentList.Add(data);
    }

    private void TrackChildOrbit(SpaceObject child)
    {
        int i = 0;
        for (; i < childList.Count; ++i)
        {
            if (childList[i].OrbitChild.GetInstanceID() == child.GetInstanceID())
            {
                //Already tracking orbit.
                return;
            }
        }

        OrbitData data = new OrbitData(objSpaceObject, child);
        childList.Add(data);
    }

    private void UnTrackParentOrbit(SpaceObject parent)
    {
        int i = 0;
        for (; i < parentList.Count; ++i)
        {
            if (parentList[i].OrbitParent.GetInstanceID() == parent.GetInstanceID())
            {
                parentList.RemoveAt(i);
                break;
            }
        }
    }

    private void UnTrackChildOrbit(SpaceObject child)
    {
        int i = 0;
        for (; i < childList.Count; ++i)
        {
            if (childList[i].OrbitChild.GetInstanceID() == child.GetInstanceID())
            {
                childList.RemoveAt(i);
                break;
            }
        }
    }

    private void UnTrackOrbitRelationship(SpaceObject another)
    {
        UnTrackParentOrbit(another);
        UnTrackChildOrbit(another);
    }

    private bool IsTrackingChildOrbit(SpaceObject child)
    {
        int i = 0;
        for (; i < childList.Count; ++i)
        {
            if (childList[i].OrbitChild.GetInstanceID() == child.GetInstanceID())
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTrackingParentOrbit(SpaceObject parent)
    {
        int i = 0;
        for (; i < parentList.Count; ++i)
        {
            if (parentList[i].OrbitParent.GetInstanceID() == parent.GetInstanceID())
            {
                return true;
            }
        }

        return false;
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
