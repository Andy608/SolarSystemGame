using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ManageRelativity : ManagerBase<ManageRelativity>
    {
        //private List<SpaceObject> objectsInUniverse;
        //private SpaceObject cameraTarget;

        //private Vector2 translationOffset;
        //private Vector2 velocityOffset;

        //private void OnEnable()
        //{
        //    CameraMove.OnCameraTargetChanged += CenterUniverse;
        //}

        //private void OnDisable()
        //{
        //    CameraMove.OnCameraTargetChanged -= CenterUniverse;
        //}

        //private void FixedUpdate()
        //{
        //    objectsInUniverse = ObjectTracker.Instance.ObjectsInUniverse;
        //    cameraTarget = CameraState.Instance.CameraMove.Target;

        //    if (cameraTarget)
        //    {
        //        velocityOffset = -CameraState.Instance.CameraMove.Target.objRigidbody.velocity;
        //        //Debug.Log(translationOffset);

        //        //Debug.Log("FIXED UPDATE: " + translationOffset);

        //        foreach (SpaceObject obj in objectsInUniverse)
        //        {
        //            obj.objRigidbody.velocity += velocityOffset;
        //        }
        //    }
        //}

        //private void CenterUniverse()
        //{
        //    cameraTarget = CameraState.Instance.CameraMove.Target;
        //    objectsInUniverse = ObjectTracker.Instance.ObjectsInUniverse;

        //    if (cameraTarget)
        //    {
        //        translationOffset = -cameraTarget.transform.position;

        //        foreach (SpaceObject obj in objectsInUniverse)
        //        {
        //            obj.transform.Translate(translationOffset);
        //        }

        //        //Debug.Log("TRANSLATE CAMERA: " + translationOffset);
        //        CameraState.Instance.CameraMove.gameObject.transform.Translate(translationOffset);
        //    }
        //}
    }
}
