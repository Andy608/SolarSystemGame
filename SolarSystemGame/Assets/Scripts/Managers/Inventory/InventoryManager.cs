using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class InventoryManager : ManagerBase<InventoryManager>
    {
        //Holds the list of ui panels
        private List<SpaceObjectUI> spaceObjPanels = new List<SpaceObjectUI>();

        //Top UI
        [SerializeField] private Text playerMoneyLabel;
        [SerializeField] private Button settingsButton;

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
            UnlockablesManager.OnUnlockNewObject += UnlockUIPanel;
        }

        private void OnDisable()
        {
            UnlockablesManager.OnUnlockNewObject -= UnlockUIPanel;
        }

        //Call this when the player clicks the OK button in the popup gui explaining the newly unlocked obj
        private void UnlockUIPanel(EnumObjectType type)
        {
            foreach (SpaceObjectUI currentPanel in spaceObjPanels)
            {
                if (currentPanel.ObjectInfo.Type == type)
                {
                    Debug.Log("Unlock the UI Panel for type: " + type.ToString());
                    currentPanel.IsUnlocked = true;
                    break;
                }
            }
        }
    }
}
