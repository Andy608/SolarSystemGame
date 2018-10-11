﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UnlockablesManager : ManagerBase<UnlockablesManager>
    {
        public delegate void UnlockNewObject(EnumObjectType type);
        public static event UnlockNewObject OnUnlockNewObject;
        
        //Holds the list of unlocked items.
        private List<EnumObjectType> unlockedObjectList = new List<EnumObjectType>();

        public List<EnumObjectType> UnlockedSpaceObjects { get { return unlockedObjectList; } }

        private void OnEnable()
        {
            PhysicsProperties.OnAbsorbed += SpaceObjectAbsorbed;
        }

        private void OnDisable()
        {
            PhysicsProperties.OnAbsorbed -= SpaceObjectAbsorbed;
        }

        private void Start()
        {
            InitStartingUnlockedObjects();
        }

        public bool IsObjectUnlocked(EnumObjectType type)
        {
            return unlockedObjectList.Contains(type);
        }

        private void SpaceObjectAbsorbed(SpaceObject absorber, SpaceObject absorbed)
        {
            const string TAG_ASTEROID = "Asteroid";
            if (absorber.tag == TAG_ASTEROID && absorbed.tag == TAG_ASTEROID)
            {
                UnlockTerrestrialPlanet();
            }
        }

        private void UnlockObject(EnumObjectType type)
        {
            if (!IsObjectUnlocked(type))
            {
                Debug.Log("Unlocked new object type! Type: " + type.ToString());
                unlockedObjectList.Add(type);

                if (OnUnlockNewObject != null)
                {
                    OnUnlockNewObject(type);
                }
            }
        }

        private void InitStartingUnlockedObjects()
        {
            Dictionary<EnumObjectType, SpaceObjectType> spaceObjTypeList = ObjectStore.Instance.SpaceObjTypeList;

            foreach (KeyValuePair<EnumObjectType, SpaceObjectType> currentSpaceObj in spaceObjTypeList)
            {
                Debug.Log("Unlocked new object type! Type: " + currentSpaceObj.Key.ToString());

                if (OnUnlockNewObject != null)
                {
                    OnUnlockNewObject(currentSpaceObj.Key);
                }
            }
        }

        public void UnlockTerrestrialPlanet()
        {
            UnlockObject(EnumObjectType.TERRESTRIAL_PLANET);
        }

        public void UnlockGasPlanet()
        {
            UnlockObject(EnumObjectType.GAS_PLANET);
        }
    }
}
