using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class CameraState : ManagerBase<CameraState>
    {
        public delegate void FollowBiggestAction();
        public static event FollowBiggestAction OnFollowBiggest;

        public delegate void FollowSelectedAction();
        public static event FollowSelectedAction OnFollowSelected;

        public delegate void FollowAverageAction();
        public static event FollowAverageAction OnFollowAverage;

        public delegate void NoFollowAction();
        public static event NoFollowAction OnNoFollow;

        public enum EnumCameraState
        {
            //Follows the most massive object in the scene
            FOLLOW_BIGGEST,

            //Follows the selected object in the scene.
            FOLLOW_SELECTED,

            //Calculates the average of all the objects in the scene and follows that point.
            FOLLOW_AVERAGE,

            //Does not follow an object, the player can use two fingers to move around.
            NO_FOLLOW
        }

        [SerializeField] private EnumCameraState currentCameraState = EnumCameraState.FOLLOW_BIGGEST;

        [HideInInspector] public Camera gameCamera;
        [HideInInspector] public CameraMove objCameraMove;
        [HideInInspector] public CameraZoom objCameraZoom;

        private void Start()
        {
            gameCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            objCameraMove = gameCamera.GetComponent<CameraMove>();
            objCameraZoom = gameCamera.GetComponent<CameraZoom>();
            FireCameraStateEvent();
        }

        private void OnValidate()
        {
            FireCameraStateEvent();
        }

        private void OnEnable()
        {
            ObjectTracker.OnObjectSpawned += OnObjectSpawned;

            PhysicsProperties.OnAbsorbed += OnBiggestTargetAbsorbed;
            ObjectTracker.OnObjectSpawned += OnBiggestTargetUpdate;

            PhysicsProperties.OnAbsorbed += OnSelectedTargetAbsorbed;
            ObjectTracker.OnSelectedObjectChanged += OnSelectedTargetChanged;
        }

        private void OnDisable()
        {
            ObjectTracker.OnObjectSpawned -= OnObjectSpawned;

            PhysicsProperties.OnAbsorbed -= OnBiggestTargetAbsorbed;
            ObjectTracker.OnObjectSpawned -= OnBiggestTargetUpdate;

            PhysicsProperties.OnAbsorbed -= OnSelectedTargetAbsorbed;
            ObjectTracker.OnSelectedObjectChanged -= OnSelectedTargetChanged;
        }

        public EnumCameraState CurrentCameraState
        {
            get
            {
                return currentCameraState;
            }

            set
            {
                if (currentCameraState != value)
                {
                    currentCameraState = value;
                    FireCameraStateEvent();
                }
            }
        }

        public bool IsState(EnumCameraState state)
        {
            return currentCameraState == state;
        }

        public void OnObjectSpawned(SpaceObject spawnedObj)
        {
            //SpaceObject target = objCameraMove.ObjTarget;

            //if (currentCameraState != EnumCameraState.NO_FOLLOW && target)
            //{
            //    spawnedObj.objRigidbody.velocity = target.objRigidbody.velocity;
            //    spawnedObj.objRigidbody.AddRelativeForce(target.objPhysicsProperties.acceleration);
            //}
        }

        public void OnBiggestTargetAbsorbed(SpaceObject absorber)
        {
            if (currentCameraState == EnumCameraState.FOLLOW_BIGGEST)
            {
                FireCameraStateEvent();
            }
        }

        public void OnBiggestTargetUpdate(SpaceObject spawnedObj)
        {
            if (currentCameraState == EnumCameraState.FOLLOW_BIGGEST)
            {
                FireCameraStateEvent();
            }
        }

        public void OnSelectedTargetAbsorbed(SpaceObject absorber)
        {
            if (currentCameraState == EnumCameraState.FOLLOW_SELECTED)
            {
                if (absorber == objCameraMove.ObjTarget)
                {
                    //Selected got absorbed so we don't follow the selected anymore.
                    objCameraMove.ObjTarget = absorber;
                }
            }
        }

        public void OnSelectedTargetChanged()
        {
            if (currentCameraState == EnumCameraState.FOLLOW_SELECTED)
            {
                FireCameraStateEvent();
            }
        }

        private void FireCameraStateEvent()
        {
            if (currentCameraState == EnumCameraState.FOLLOW_BIGGEST)
            {
                if (OnFollowBiggest != null)
                {
                    Debug.Log("BIGGEST FIRED");
                    OnFollowBiggest();
                }
            }
            else if (currentCameraState == EnumCameraState.FOLLOW_SELECTED)
            {
                if (OnFollowSelected != null)
                {
                    Debug.Log("SELECTED FIRED");
                    OnFollowSelected();
                }
            }
            else if (currentCameraState == EnumCameraState.FOLLOW_AVERAGE)
            {
                if (OnFollowAverage != null)
                {
                    OnFollowAverage();
                }
            }
            else if (currentCameraState == EnumCameraState.NO_FOLLOW)
            {
                if (OnNoFollow != null)
                {
                    OnNoFollow();
                }
            }
        }
    }
}
