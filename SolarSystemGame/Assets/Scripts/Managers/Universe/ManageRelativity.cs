using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ManageRelativity : ManagerBase<ManageRelativity>
    {
        private GameObject cameraTarget;

        private Vector2 translationOffset;
        private Vector2 velocityOffset;

        private void OnEnable()
        {
            //CameraState.OnCameraTargetChanged += CenterUniverse;
            //CameraState.OnCameraStateChange += CenterUniverse;
        }

        private void OnDisable()
        {
            //CameraState.OnCameraTargetChanged -= CenterUniverse;
            //CameraState.OnCameraStateChange -= CenterUniverse;
        }

        private void FixedUpdate()
        {
            cameraTarget = CameraState.Instance.Target;

            if (cameraTarget)
            {
                velocityOffset = -CameraState.Instance.TargetObjVelocity;

                foreach (SpaceObject obj in ObjectTracker.Instance.ObjectsInUniverse)
                {
                    obj.objRigidbody.velocity += velocityOffset;
                }
            }

            CenterUniverse();
        }

        //private void CenterUniverse(EnumCameraFollow followType)
        //{
        //    CenterUniverse();
        //}

        private void CenterUniverse()
        {
            cameraTarget = CameraState.Instance.Target;

            if (cameraTarget)
            {
                translationOffset = -cameraTarget.transform.position;

                foreach (SpaceObject obj in ObjectTracker.Instance.ObjectsInUniverse)
                {
                    //if (obj.objOrbitTrail != null)
                    //{
                        //Debug.Log("HELLO");
                        //obj.objOrbitTrail.ObjTrailRenderer.Clear();
                        //obj.objOrbitTrail.ObjTrailRenderer.transform.Translate(translationOffset);
                    //}

                    obj.transform.Translate(translationOffset);
                }

                CameraState.Instance.TranslateCamera(translationOffset);
            }
        }
    }
}
