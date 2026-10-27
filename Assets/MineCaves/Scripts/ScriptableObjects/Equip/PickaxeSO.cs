using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickaxe", menuName = "Scriptable Objects/Mining/Pickaxe")]
public class PickaxeSO : ScriptableObject
{
    public string code;

    public int damage;
    public float bonusSpeed;
    public int shopTier;

    public int price;
    public int maxEnchantsCount = 4;

    public List<CraftIngredient> craftIngredients = new List<CraftIngredient>();
    public PickaxeAbilitySO pickaxeAbility;
}

[System.Serializable]
public struct CraftIngredient
{
    public ItemData item;
    public int amount;
}
