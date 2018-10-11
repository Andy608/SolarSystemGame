using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class InventoryManager : ManagerBase<InventoryManager>
    {
        public delegate void SpaceObjectUIChange(SpaceObjectUI spaceObjUI);
        public static event SpaceObjectUIChange OnSpaceObjectUIChange;

        [SerializeField] private SpaceObjectUI prefabSpaceObjUI;
        [SerializeField] private SpaceObjectUnlockPopupUI prefabSpaceObjPopupUI;

        [SerializeField] private Canvas mainCanvas;

        //Holds the list of ui panels
        private List<SpaceObjectUI> spaceObjPanels = new List<SpaceObjectUI>();

        public List<SpaceObjectUI> SpaceObjectPanels { get { return spaceObjPanels; } }

        //The popup ui
        private SpaceObjectUnlockPopupUI popupUI;

        //Header UI
        [SerializeField] private Text playerMoneyLabel;
        [SerializeField] private Button settingsButton;

        //Space Obj List UI
        [SerializeField] private RectTransform objListContentPanel;

        private void Start()
        {
            popupUI = Instantiate(prefabSpaceObjPopupUI, mainCanvas.transform, false);
            popupUI.gameObject.SetActive(false);

            spaceObjPanels.Clear();
            PopulateUIList();

            UpdateFundsUI(MoneyManager.Instance.Funds);
        }

        private void OnEnable()
        {
            UnlockablesManager.OnUnlockNewObject += UnlockUIPanelPopup;
            OnSpaceObjectUIChange += UpdateUIListElement;
            SpaceObjectUnlockPopupUI.OnSpaceObjectUIPopupConfirmed += UnlockUIPanel;
            MoneyManager.OnFundsChanged += UpdateFundsUI;
        }

        private void OnDisable()
        {
            UnlockablesManager.OnUnlockNewObject -= UnlockUIPanelPopup;
            OnSpaceObjectUIChange -= UpdateUIListElement;
            SpaceObjectUnlockPopupUI.OnSpaceObjectUIPopupConfirmed -= UnlockUIPanel;
            MoneyManager.OnFundsChanged -= UpdateFundsUI;
        }

        public float GetCost(EnumObjectType type)
        {
            foreach (SpaceObjectUI currentPanel in spaceObjPanels)
            {
                if (currentPanel.ObjectInfo.Type == type)
                {
                    return currentPanel.ObjectInfo.CurrentMoneyPerMass * currentPanel.CurrentMass;
                }
            }

            Debug.Log("UNKNOWN OBJECT TYPE. Returning no cost: " + type);
            return 0.0f;
        }

        public float GetMass(EnumObjectType type)
        {
            foreach (SpaceObjectUI currentPanel in spaceObjPanels)
            {
                if (currentPanel.ObjectInfo.Type == type)
                {
                    return currentPanel.CurrentMass;
                }
            }

            Debug.Log("UNKNOWN OBJECT TYPE. Returning 100 mass: " + type);
            return 100;
        }

        //Call this when the player clicks the OK button in the popup gui explaining the newly unlocked obj
        private void UnlockUIPanel(EnumObjectType type)
        {
            foreach (SpaceObjectUI currentPanel in spaceObjPanels)
            {
                if (currentPanel.ObjectInfo.Type == type)
                {
                    Debug.Log("Unlock the UI Panel for type: " + currentPanel.ObjectInfo.Type.ToString());
                    currentPanel.ObjectInfo.IsUnlocked = true;

                    if (OnSpaceObjectUIChange != null)
                    {
                        OnSpaceObjectUIChange(currentPanel);
                    }
                }
            }
        }

        private void UnlockUIPanelPopup(EnumObjectType type)
        {
            foreach (SpaceObjectUI currentPanel in spaceObjPanels)
            {
                Debug.Log("Type: " + type);
                if (currentPanel.ObjectInfo.Type == type)
                {
                    popupUI.ObjectInfo = currentPanel.ObjectInfo;
                    popupUI.gameObject.SetActive(true);
                    break;
                }
            }
        }

        private void PopulateUIList()
        {
            Dictionary<EnumObjectType, SpaceObjectType> allObjectTypes = ObjectStore.Instance.SpaceObjTypeList;

            int index = 0;
            Vector3 position;
            Transform spaceObjTransform;

            //20 unit spacing
            //720 unit width
            //380 unit height
            float UI_WIDTH = prefabSpaceObjUI.GetComponent<RectTransform>().rect.width;
            float UI_HEIGHT = prefabSpaceObjUI.GetComponent<RectTransform>().rect.height;
            float SPACING = 20.0f;

            foreach (SpaceObjectType type in allObjectTypes.Values)
            {
                SpaceObjectUI spaceObjUI = Instantiate(prefabSpaceObjUI, objListContentPanel, false);
                spaceObjUI.ObjectInfo = type;
                spaceObjTransform = spaceObjUI.transform;

                position = spaceObjTransform.localPosition;

                //This makes gaps between two UIs 2 * SPACING. If we don't like that we can fix it another time.
                position.x = (index * (UI_WIDTH + SPACING)) + SPACING;
                position.y = 0.0f;

                spaceObjTransform.SetParent(objListContentPanel);
                spaceObjTransform.localPosition = position;

                spaceObjPanels.Add(spaceObjUI);
                ++index;
            }

            //Maybe
            Vector2 sizeDelta = objListContentPanel.sizeDelta;

            //This makes gaps between two UIs 2 * SPACING. If we don't like that we can fix it another time.
            sizeDelta.x = (index * (UI_WIDTH + SPACING)) + SPACING;
            sizeDelta.y = UI_HEIGHT;

            objListContentPanel.sizeDelta = sizeDelta;

            Debug.Log("SPACE PANEL SIZE: " + spaceObjPanels.Count);
        }

        private void UpdateUIListElement(SpaceObjectUI spaceObjUI)
        {
            spaceObjUI.UpdateUI();
        }

        private void UpdateFundsUI(float funds)
        {
            //Eventually convert funds to 10a 10b 10c etc.
            playerMoneyLabel.text = funds.ToString();
        }
    }
}
