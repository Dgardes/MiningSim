using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enchantment", menuName = "Scriptable Objects/Mining/Enchantment")]
public class EnchantmerntSO : ScriptableObject
{
    public string code;
    public int price;

    public EffectDisplaySO effectDisplay;

    public List<BlockEffect> blockEffects = new List<BlockEffect>();
    public List<StatModifier> modifiers = new List<StatModifier>();
}
