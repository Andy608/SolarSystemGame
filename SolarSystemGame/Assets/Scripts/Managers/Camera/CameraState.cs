using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumCameraFollow
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

namespace Managers
{
    public class CameraState : ManagerBase<CameraState>
    {
        public delegate void CameraTargetChangedAction();
        public static event CameraTargetChangedAction OnCameraTargetChanged;

        public delegate void CameraStateChangeAction(EnumCameraFollow cameraFollowType);
        public static event CameraStateChangeAction OnCameraStateChange;

        [SerializeField] private EnumCameraFollow currentCameraFollowType = EnumCameraFollow.FOLLOW_BIGGEST;
        [SerializeField] private Camera gameCamera;
        private Transform gameCameraTransform;

        private CameraZoom objCameraZoom;

        private GameObject objTarget;
        private Transform objTargetTransform;
        private Vector3 objPrevPosition;
        private Vector3 objVelocity;


        private Vector3 currentPosition;
        private Vector3 targetPosition = Vector3.zero;

        //Make camera smooth in the future. Come back to this
        //private Vector3 followVelocity = Vector3.zero;
        //private float smoothTime = 0.6f;

        private GameObject noTargetObj = null;

        [SerializeField] private GameObject targetObjPrefab;

        public GameObject Target { get { return objTarget; } }
        public GameObject NoTargetObj { get { return noTargetObj; } }
        public CameraZoom CameraZoom { get { return objCameraZoom; } }
        public Vector3 TargetObjVelocity { get { return objVelocity; } }

        private void Start()
        {
            InitTargetObjects();
            objTarget = noTargetObj;

            objTargetTransform = objTarget.transform;
            objPrevPosition = objTargetTransform.position;
            objVelocity = Vector3.zero;

            gameCameraTransform = gameCamera.transform;

            objCameraZoom = gameCamera.GetComponent<CameraZoom>();
            FireStateChangedEvent();
        }

        private void OnValidate()
        {
            FireStateChangedEvent();
        }

        private void OnEnable()
        {
            ObjectTracker.OnObjectSpawned += OnObjectSpawned;
            UniversePlaySpaceManager.OnObjectDestroyed += OnObjectDestroyed;
            ObjectTracker.OnSelectedObjectChanged += FireStateChangedEvent;

            OnCameraStateChange += SetObjectTarget;
        }

        private void OnDisable()
        {
            ObjectTracker.OnObjectSpawned -= OnObjectSpawned;
            UniversePlaySpaceManager.OnObjectDestroyed -= OnObjectDestroyed;
            ObjectTracker.OnSelectedObjectChanged -= FireStateChangedEvent;

            OnCameraStateChange -= SetObjectTarget;
        }

        private void InitTargetObjects()
        {
            noTargetObj = Instantiate(targetObjPrefab);
            noTargetObj.name = "(NoTarget) Camera Target";
            noTargetObj.SetActive(false);
        }

        private void FixedUpdate()
        {
            objVelocity = objTargetTransform.position - objPrevPosition;

            currentPosition = gameCameraTransform.position;
            targetPosition = objTargetTransform.position;
            targetPosition.z = currentPosition.z;

            //In the future maybe change this to ArriveSteering
            currentPosition += (targetPosition - currentPosition) * 0.1f;

            gameCameraTransform.position = currentPosition;

            objPrevPosition = objTargetTransform.position;
        }
        
        public void TranslateCamera(Vector2 newPos)
        {
            gameCameraTransform.Translate(newPos);
            objPrevPosition = gameCameraTransform.transform.position;
        }

        public void UpdateNoTargetObject(Vector3 offset)
        {
            noTargetObj.transform.position += offset;
        }

        private void SetObjectTarget(EnumCameraFollow cameraState)
        {
            GameObject newTarget;
            SpaceObject temp;

            switch (cameraState)
            {
                case EnumCameraFollow.FOLLOW_BIGGEST:
                    temp = Managers.ObjectTracker.Instance.MostMassiveObj;
                    newTarget = temp ? temp.gameObject : noTargetObj;
                    break;
                case EnumCameraFollow.FOLLOW_SELECTED:
                    temp = Managers.ObjectTracker.Instance.SelectedObj;
                    newTarget = temp ? temp.gameObject : noTargetObj;
                    break;
                case EnumCameraFollow.FOLLOW_AVERAGE:
                    newTarget = Managers.UniversePlaySpaceManager.Instance.CenterOfUniverse;
                    break;
                case EnumCameraFollow.NO_FOLLOW:
                default:
                    newTarget = noTargetObj;
                    break;
            }

            if (objTarget != newTarget)
            {
                if (!newTarget)
                {
                    Debug.Log("There is 0 objs in the universe. :o");
                    SetTarget(noTargetObj);
                }
                else
                {
                    SetTarget(newTarget);
                }

                objTargetTransform = objTarget.transform;

                if (OnCameraTargetChanged != null)
                {
                    OnCameraTargetChanged();
                }
            }
        }

        public EnumCameraFollow CurrentCameraState
        {
            get
            {
                return currentCameraFollowType;
            }

            set
            {
                if (currentCameraFollowType != value)
                {
                    currentCameraFollowType = value;
                    FireStateChangedEvent();
                }
            }
        }

        public bool IsState(EnumCameraFollow state)
        {
            return currentCameraFollowType == state;
        }

        public void OnObjectSpawned(SpaceObject spawnedObj)
        {
            if (currentCameraFollowType == EnumCameraFollow.FOLLOW_BIGGEST)
            {
                FireStateChangedEvent();
            }
        }

        public void OnObjectDestroyed(SpaceObject destroyedObj)
        {
            if (currentCameraFollowType == EnumCameraFollow.FOLLOW_SELECTED)
            {
                if (destroyedObj.transform == objTarget)
                {
                    //Selected got absorbed so we don't follow the selected anymore.
                    SetTarget(noTargetObj);
                }
            }
            else if (currentCameraFollowType == EnumCameraFollow.FOLLOW_BIGGEST ||
                currentCameraFollowType == EnumCameraFollow.FOLLOW_AVERAGE)
            {
                FireStateChangedEvent();
            }
        }

        private void FireStateChangedEvent()
        {
            if (OnCameraStateChange != null)
            {
                OnCameraStateChange(currentCameraFollowType);
            }
        }

        private void SetTarget(GameObject target)
        {
            objTarget = target;
            objTargetTransform = objTarget.transform;
        }
    }
}
