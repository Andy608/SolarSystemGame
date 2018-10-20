using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UniverseSetupManager : ManagerBase<UniverseSetupManager>
    {
        [SerializeField] private SpaceObjectType startingObjectType;
        private SpaceObject startingObjPrefab;

        //private SpaceObject startingUniverseObj;

        private void Start()
        {
            startingObjPrefab = ObjectStore.Instance.GetSpaceObjectPrefab(startingObjectType);

            if (startingObjPrefab)
            {
                InitAndSpawnStartingObject(startingObjPrefab);
            }
            else
            {
                Debug.Log("The starting object should never be null. Setting to default: " + EnumObjectType.PROTO_STAR.ToString());
                startingObjPrefab = ObjectStore.Instance.GetSpaceObjectPrefab(EnumObjectType.PROTO_STAR);
                InitAndSpawnStartingObject(startingObjPrefab);

                //Make an event manager class that holds all the events. Then tell the event manager you want to call a certain event.
                //if (ObjectTracker.OnObjectSpawned != null)
                //{
                //    ObjectTracker.OnObjectSpawned();
                //}
            }
        }

        private void InitAndSpawnStartingObject(SpaceObject startingObj)
        {
            //Do a cool timeline animation thing with this to zoom in to the solar system.
            /*startingUniverseObj = */Instantiate(startingObj);
        }
    }
}