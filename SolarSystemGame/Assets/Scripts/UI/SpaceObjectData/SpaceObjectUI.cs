using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceObjectUI : MonoBehaviour
{
    //General
    private bool isUnlocked;

    //Left Column UI
    [SerializeField] private Text objectName;
    [SerializeField] private Image objectImage;
    [SerializeField] private Text objectCostLabel;
    [SerializeField] private Text objectMassLabel;

    //Right Colum UI
    [SerializeField] private Text objectDescription;

    [SerializeField] private Slider massSlider;
    [SerializeField] private float currentMass;

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

    public bool IsUnlocked
    {
        get
        {
            return isUnlocked;
        }

        set
        {
            isUnlocked = value;
        }
    }

    //Update UI when the object is set for this ui, 
    //and when events for the slider is changed -> update mass and cost, 
    //or when a type of that kind is bought -> update cost.
    //These variables get changed in SpaceObjectType and then the ui is updated here.
    private void UpdateUI()
    {
        currentMass = objectInfo.DefaultMass;

        //Left Column UI
        objectName.text = objectInfo.Name;
        objectImage.sprite = objectInfo.Sprite;
        objectCostLabel.text = GetCost().ToString();
        objectMassLabel.text = currentMass.ToString();

        //Right Column UI
        objectDescription.text = objectInfo.Description;
    }

    private float GetCost()
    {
        if (objectInfo)
        {
            return objectInfo.CurrentMoneyPerMass * currentMass;
        }
        else
        {
            Debug.Log("objectType SHOULDN'T BE NULL");
            return 0.0f;
        }
    }
}
