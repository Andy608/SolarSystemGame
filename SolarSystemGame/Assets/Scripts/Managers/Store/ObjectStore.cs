using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumObjectType
{
    ASTEROID,
    TERRESTRIAL_PLANET,
    GAS_PLANET
}

namespace Managers
{
    public class ObjectStore : ManagerBase<ObjectStore>
    {
        [SerializeField] private List<SpaceObject> spaceObjectList = new List<SpaceObject>();

        private Dictionary<EnumObjectType, SpaceObjectType> spaceObjTypeList = new Dictionary<EnumObjectType, SpaceObjectType>();

        public Dictionary<EnumObjectType, SpaceObjectType> SpaceObjTypeList
        {
            get
            {
                return spaceObjTypeList;
            }
        }

        private void Start()
        {
            Debug.Log("Initializing space object dictionary.");

            foreach (SpaceObject currentSpaceObj in spaceObjectList)
            {
                Debug.Log("Adding type: " + currentSpaceObj.objSpaceObjectType.Type.ToString());
                SpaceObjectType currentRef = Instantiate(currentSpaceObj.objSpaceObjectType);

                spaceObjTypeList.Add(currentRef.Type, currentRef);
            }
        }

        public SpaceObjectType GetByType(EnumObjectType type)
        {
            SpaceObjectType obj = null;
            spaceObjTypeList.TryGetValue(type, out obj);
            return obj;
        }

        public SpaceObject GetSpaceObjectPrefab(EnumObjectType type)
        {
            foreach (SpaceObject currentSpaceObj in spaceObjectList)
            {
                if (currentSpaceObj.objSpaceObjectType.Type == type)
                {
                    return currentSpaceObj;
                }
            }

            return null;
        }
    }
}
