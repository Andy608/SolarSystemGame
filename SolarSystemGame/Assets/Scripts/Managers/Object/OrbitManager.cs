using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class OrbitManager : ManagerBase<OrbitManager>
    {
        private float orbitCount;

        [SerializeField] private Text orbitLabel;

        private void OnEnable()
        {
            OrbitData.OnOrbitOccured += TrackOrbit;
        }

        private void OnDisable()
        {
            OrbitData.OnOrbitOccured -= TrackOrbit;
        }

        private void TrackOrbit(SpaceObject parent, SpaceObject orbital)
        {
            //For now.
            ++orbitCount;
            orbitLabel.text = orbitCount.ToString();

            if (orbitCount == 5)
            {
                UnlockablesManager.Instance.UnlockGasPlanet();
            }

            if (orbitCount == 10)
            {
                //You win! (Temporary)
                InventoryManager.Instance.ShowWinState();
            }
        }
    }
}