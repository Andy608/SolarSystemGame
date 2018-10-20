using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumObjectType
{
    //Misc
    ASTEROID,
    COMET,
    BLACK_HOLE,

    //Planets
    DWARF_PLANET,
    OCEAN_PLANET,
    IRON_PLANET,
    ICE_PLANET,
    LAVA_PLANET,
    EARTH_LIKE_PLANET,
    GAS_PLANET,
    TERRESTRIAL_PLANET,
    GAS_GIANT,

    //Stars In order of lifetime
    PROTO_STAR,
    T_TAURI_STAR,
    MAIN_SEQUENCE_STAR,
    RED_GIANT,
    WHITE_GIANT,
    SUPER_GIANT,
    WHITE_DWARF,
    RED_DWARF,
    NEUTRON_STAR
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
                //Debug.Log("Adding type: " + currentSpaceObj.objSpaceObjectType.Type.ToString());
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

        public SpaceObject GetSpaceObjectPrefab(SpaceObjectType type)
        {
            foreach (SpaceObject currentSpaceObj in spaceObjectList)
            {
                if (currentSpaceObj.objSpaceObjectType == type)
                {
                    return currentSpaceObj;
                }
            }

            return null;
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
