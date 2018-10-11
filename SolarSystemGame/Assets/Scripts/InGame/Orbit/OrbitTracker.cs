using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shouldn't be called OrbitTracker. Should be called SpaceObjectOrbit
[RequireComponent(typeof(SpaceObject))]
public class OrbitTracker : MonoBehaviour
{
    private SpaceObject objSpaceObject;

    private List<OrbitData> childList = new List<OrbitData>();

    public List<OrbitData> ChildList
    {
        get
        {
            return childList;
        }
    }

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
        if (parent == objSpaceObject)
        {
            TrackOrbit(child);
        }
    }

    private void UnTrackFromAbsorption(SpaceObject parent, SpaceObject child)
    {
        UnTrackOrbit(child);

        if (parent.objRigidbody.mass > objSpaceObject.objRigidbody.mass)
        {
            UnTrackOrbit(parent);
        }
    }

    private void UnTrackFromGravitationalPull(SpaceObject parent, SpaceObject child)
    {
        if (parent == objSpaceObject)
        {
            UnTrackOrbit(child);
        }
    }

    public void TrackOrbit(SpaceObject orbiter)
    {
        int i = 0;

        for (; i < childList.Count; ++i)
        {
            if (childList[i].OrbitChild.GetInstanceID() == orbiter.GetInstanceID())
            {
                //Already tracking orbit.
                return;
            }
        }

        OrbitData data = new OrbitData(objSpaceObject, orbiter);
        childList.Add(data);
    }

    public void UnTrackOrbit(SpaceObject orbiter)
    {
        int i = 0;

        for (; i < childList.Count; ++i)
        {
            if (childList[i].OrbitChild.GetInstanceID() == orbiter.GetInstanceID())
            {
                childList.RemoveAt(i);
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
