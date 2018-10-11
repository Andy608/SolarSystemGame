using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MoneyManager : ManagerBase<MoneyManager>
    {
        public delegate void FundsChanged(float funds);
        public static event FundsChanged OnFundsChanged;

        private float funds = 100.0f;

        public float Funds
        {
            get
            {
                return funds;
            }
        }

        private void OnEnable()
        {
            OrbitData.OnOrbitOccured += IncreaseFunds;
            ObjectTracker.OnObjectSpawned += DecreaseFunds;
        }

        private void OnDisable()
        {
            OrbitData.OnOrbitOccured -= IncreaseFunds;
            ObjectTracker.OnObjectSpawned -= DecreaseFunds;
        }

        private void IncreaseFunds(SpaceObject parent, SpaceObject orbital)
        {
            //Eventually each SpaceObjectType will have a unique amount of money to increase per orbit.
            funds += 50.0f;

            if (OnFundsChanged != null)
            {
                OnFundsChanged(funds);
            }
        }

        private void DecreaseFunds(SpaceObject objSpawned)
        {
            funds -= InventoryManager.Instance.GetCost(objSpawned.objSpaceObjectType.Type);

            if (OnFundsChanged != null)
            {
                OnFundsChanged(funds);
            }

            CheckIfBankrupt();
        }

        private void CheckIfBankrupt()
        {
            List<SpaceObjectUI> spaceObjPanels = InventoryManager.Instance.SpaceObjectPanels;
            SpaceObjectUI currentPanel;
            float minimumCost = 0.0f;

            for (int i = 0; i < spaceObjPanels.Count; ++i)
            {
                currentPanel = spaceObjPanels[i];

                if (i == 0)
                {
                    minimumCost = currentPanel.ObjectInfo.CurrentMoneyPerMass * currentPanel.ObjectInfo.DefaultMass;
                }
                else
                {
                    minimumCost = Mathf.Min(minimumCost, currentPanel.ObjectInfo.CurrentMoneyPerMass * currentPanel.ObjectInfo.DefaultMass);
                }
            }

            if (funds < minimumCost)
            {
                Debug.Log("We're backrupt!");
            }
        }
    }
}

