using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceObjectUnlockPopupUI : MonoBehaviour
{
    public delegate void SpaceObjectUIPopupConfirmed(EnumObjectType spaceObjType);
    public static event SpaceObjectUIPopupConfirmed OnSpaceObjectUIPopupConfirmed;

    [SerializeField] private Text objectName;
    [SerializeField] private Image objectImage;
    [SerializeField] private Text objectDescription;

    [SerializeField] private Button okayButton;

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

    public void UpdateUI()
    {
        objectName.text = objectInfo.Name;
        objectImage.sprite = objectInfo.Sprite;
        objectDescription.text = objectInfo.Description;
    }

    public void OnOKClicked()
    {
        if (OnSpaceObjectUIPopupConfirmed != null)
        {
            OnSpaceObjectUIPopupConfirmed(objectInfo.Type);
        }

        //Managers.GameState.Instance.RequestUnpause();

        StartCoroutine(Managers.InventoryManager.Instance.ClosePopup(gameObject));
    }
}
