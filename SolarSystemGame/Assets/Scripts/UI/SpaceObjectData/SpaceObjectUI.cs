using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceObjectUI : MonoBehaviour
{
    public delegate void UISelectionUpdated(EnumObjectType type, float cost, float mass);
    public static event UISelectionUpdated OnUISelectionUpdated;

    public delegate void UIObjectSelected(EnumObjectType type, float cost, float mass);
    public static event UIObjectSelected OnUIObjectSelected;

    //Panels
    [SerializeField] private RectTransform lockedPanel;
    [SerializeField] private RectTransform unlockedPanel;

    //Left Column UI
    [SerializeField] private Text objectName;
    [SerializeField] private Image objectImage;
    [SerializeField] private Text objectCostLabel;
    [SerializeField] private Text objectMassLabel;

    //Right Colum UI
    [SerializeField] private Text objectDescription;

    [SerializeField] private Slider massSlider;
    private float currentMass;

    [SerializeField] private Button selectButton;

    private SpaceObjectType objectInfo;

    public SpaceObjectType ObjectInfo
    {
        get
        {
            return objectInfo;
        }

        set
        {
            objectInfo = value;
            UpdateUI();
        }
    }

    public float CurrentMass { get { return currentMass; } }

    //Update UI when the object is set for this ui, 
    //and when events for the slider is changed -> update mass and cost, 
    //or when a type of that kind is bought -> update cost.
    //These variables get changed in SpaceObjectType and then the ui is updated here.
    public void UpdateUI()
    {
        massSlider.minValue = objectInfo.DefaultMass;
        massSlider.maxValue = objectInfo.MaxMass;
        currentMass = Mathf.RoundToInt(massSlider.value);

        //Left Column UI
        objectName.text = objectInfo.Name;
        objectImage.sprite = objectInfo.Sprite;
        objectCostLabel.text = GetCost().ToString();
        objectMassLabel.text = currentMass.ToString();

        //Right Column UI
        objectDescription.text = objectInfo.Description;

        if (objectInfo.IsUnlocked)
        {
            lockedPanel.gameObject.SetActive(false);
            unlockedPanel.gameObject.SetActive(true);
        }
        else
        {
            lockedPanel.gameObject.SetActive(true);
            unlockedPanel.gameObject.SetActive(false);
        }

        if (OnUISelectionUpdated != null)
        {
            OnUISelectionUpdated(objectInfo.Type, GetCost(), currentMass);
        }
    }

    private float GetCost()
    {
        return objectInfo.CurrentMoneyPerMass * CurrentMass;
    }

    //When the button to select a space object in the ui is clicked.
    public void OnUISelectionClicked()
    {
       // Managers.InventoryManager.Instance.ObjectToSpawn = Managers.ObjectStore.Instance.GetSpaceObjectPrefab(objectInfo.Type);
        Debug.Log("UPDATED UI SLECtiON FOR TyPE: " + objectInfo.Type.ToString());
        UpdateUI();

        if (OnUIObjectSelected != null)
        {
            OnUIObjectSelected(objectInfo.Type, GetCost(), currentMass);
        }
    }
}
