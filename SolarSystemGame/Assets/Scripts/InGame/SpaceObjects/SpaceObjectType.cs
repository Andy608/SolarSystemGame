using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpaceObject", menuName = "SpaceObject")]
public class SpaceObjectType : ScriptableObject
{
    //General
    [SerializeField] private bool isUnlocked;

    [SerializeField] private EnumObjectType objectType;

    //Randomize in the future.
    [SerializeField] private Sprite sprite;

    [SerializeField] private string title;
    [SerializeField] [Multiline] private string description;

    //The amount of money each mass unit is for the space object.
    [SerializeField] private float moneyPerMass = 0.16f;
    private float currentMoneyPerMass;

    //Increases the price per mass by this much each time the space object is bought.
    [SerializeField] private float moneyPerMassMultiplier = 1.05f;

    [SerializeField] private float radiusScaleMultiplier = 1000.0f;

    [SerializeField] private float defaultMass;
    [SerializeField] private float maxMass;

    private int buyCounter;

    public EnumObjectType Type { get { return objectType; } }
    public Sprite Sprite { get { return sprite; } }
    public string Name { get { return title; } }
    public string Description { get { return description; } }
    public float RadiusScaleMult { get { return radiusScaleMultiplier; } }
    public float CurrentMoneyPerMass { get { return currentMoneyPerMass; } }
    public float PixelsPerUnit { get { return sprite.pixelsPerUnit; } }
    public float SpriteWidth { get { return sprite.texture.width; } } //Might not work with texture atlas

    public float DefaultMass { get { return defaultMass; } }
    public float MaxMass { get { return maxMass; } }
    public bool IsUnlocked { get { return isUnlocked; } set { isUnlocked = value; } }

    private void Awake()
    {
        currentMoneyPerMass = moneyPerMass;
        buyCounter = 0;
    }

    public void BuyObject()
    {
        //Just fun to have for now.
        ++buyCounter;

        currentMoneyPerMass *= moneyPerMassMultiplier;
    }
}
