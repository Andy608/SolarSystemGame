using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumObjectType
{
    ASTEROID,
    TERRESTRIAL_PLANET
}

namespace Managers
{
    public class ObjectStore : ManagerBase<ObjectStore>
    {
        [SerializeField] private List<SpaceObjectType> spaceObjectList = new List<SpaceObjectType>();

        private Dictionary<EnumObjectType, SpaceObjectType> spaceObjTypeList = new Dictionary<EnumObjectType, SpaceObjectType>();

        private void Start()
        {
            Debug.Log("Initializing space object dictionary.");

            foreach (SpaceObjectType currentType in spaceObjectList)
            {
                Debug.Log("Adding type: " + currentType.Type.ToString());
                spaceObjTypeList.Add(currentType.Type, currentType);
            }
        }

        public SpaceObjectType GetByType(EnumObjectType type)
        {
            SpaceObjectType obj = null;
            spaceObjTypeList.TryGetValue(type, out obj);
            return obj;
        }

    }
}
