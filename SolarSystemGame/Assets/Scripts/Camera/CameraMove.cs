using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //In the future we will be able to click on planets and follow them.
    //^^ The camera state will be toggled with buttons. ^^

    private SpaceObject objTarget;
    private Vector3 position;
    private Vector3 targetPosition;

    public SpaceObject ObjTarget
    {
        get
        {
            return objTarget;
        }

        set
        {
            objTarget = value;
        }
    }

    private void OnEnable()
    {
        Managers.CameraState.OnFollowBiggest    += SetBiggestTarget;
        Managers.CameraState.OnFollowSelected   += SetSelectedTarget;
        Managers.CameraState.OnFollowAverage    += SetAverageTarget;
        Managers.CameraState.OnNoFollow         += SetNoTarget;
    }

    private void OnDisable()
    {
        Managers.CameraState.OnFollowBiggest    -= SetBiggestTarget;
        Managers.CameraState.OnFollowSelected   -= SetSelectedTarget;
        Managers.CameraState.OnFollowAverage    -= SetAverageTarget;
        Managers.CameraState.OnNoFollow         -= SetNoTarget;
    }

    private void FixedUpdate()
    {
        //For now
        if (!objTarget) return;

        position = transform.position;
        targetPosition = objTarget.transform.position;
        targetPosition.z = position.z;

        //In the future maybe change this to ArriveSteering
        position = Vector3.Slerp(position, targetPosition, Mathf.Max(1.0f, objTarget.objRigidbody.velocity.magnitude) * Time.fixedDeltaTime);

        transform.position = position;
    }

    private void SetBiggestTarget()
    {
        //Go through the list of active objects in the universe and get the largest one.
        //^^ Make this a method call in ObjectTracker manager.
        SpaceObject mostMassiveObj = Managers.ObjectTracker.Instance.GetMostMassiveObject();

        //If there are no objects in the scene, then set the camera to no follow.
        if (!mostMassiveObj)
        {
            objTarget = null;
            //Managers.CameraState.Instance.CurrentCameraState = Managers.CameraState.EnumCameraState.NO_FOLLOW;
        }
        else
        {
            objTarget = mostMassiveObj;
        }
    }

    private void SetSelectedTarget()
    {
        //Get selected from ObjectTracker manager.

        SpaceObject selectedObj = Managers.ObjectTracker.Instance.SelectedObj;

        //If there are no objects in the scene, then set the camera to no follow.
        if (!selectedObj)
        {
            objTarget = null;
            //Managers.CameraState.Instance.CurrentCameraState = Managers.CameraState.EnumCameraState.NO_FOLLOW;
        }
        else
        {
            objTarget = selectedObj;
        }
    }

    private void SetAverageTarget()
    {
        //Make this a method in ObjectTracker manager as well. Average all the positions together.
    }

    private void SetNoTarget()
    {
        //Set the target to null.
        objTarget = null;
    }
}
