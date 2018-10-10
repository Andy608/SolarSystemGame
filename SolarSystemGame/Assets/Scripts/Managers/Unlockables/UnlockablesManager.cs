using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UnlockablesManager : ManagerBase<ManageRelativity>
    {
        public delegate void UnlockNewObject(EnumObjectType type);
        public static event UnlockNewObject OnUnlockNewObject;
        
        //Holds the list of unlocked items.
        [SerializeField] private List<EnumObjectType> unlockedObjectList = new List<EnumObjectType>();

        public List<EnumObjectType> UnlockedSpaceObjects { get { return unlockedObjectList; } }

        private void OnEnable()
        {
            PhysicsProperties.OnAbsorbed += SpaceObjectAbsorbed;
        }

        private void OnDisable()
        {
            PhysicsProperties.OnAbsorbed -= SpaceObjectAbsorbed;
        }

        public bool IsObjectUnlocked(EnumObjectType type)
        {
            return unlockedObjectList.Contains(type);
        }

        private void SpaceObjectAbsorbed(SpaceObject absorber, SpaceObject absorbed)
        {
            UnlockTerrestrialPlanet(absorber, absorbed);
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

        private void UnlockTerrestrialPlanet(SpaceObject absorber, SpaceObject absorbed)
        {
            Debug.Log("HELLO");
            const string TAG_ASTEROID = "Asteroid";
            if (absorber.tag == TAG_ASTEROID && absorbed.tag == TAG_ASTEROID)
            {
                UnlockObject(EnumObjectType.TERRESTRIAL_PLANET);
            }
        }
    }
}
